using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// n×mグリッドのインベントリを管理するpure C#クラス。
/// プレイヤーインベントリ・ルートボックス・倉庫など共通で使用する。
///
/// グリッド座標は左上原点(0,0)、右下が(width-1, height-1)。
/// アイテムはItemStackModelで管理し、左上座標をキーとしてDictionaryに保持する。
/// 各マスはInventoryCellがどのアイテムの左上座標を参照しているかを記録する。
/// </summary>
public class InventoryModel
{
    // -------------------------------------------------------
    // フィールド
    // -------------------------------------------------------

    private readonly int _width;
    private readonly int _height;

    /// <summary>各マスの占有状態 [x, y]</summary>
    private readonly Vector2Int?[,] _cells;

    /// <summary>左上座標 → ItemStackModel</summary>
    private readonly Dictionary<Vector2Int, ItemStackModel> _items;

    // -------------------------------------------------------
    // プロパティ
    // -------------------------------------------------------

    public int Width => _width;
    public int Height => _height;
    public IReadOnlyDictionary<Vector2Int, ItemStackModel> Items => _items;

    // -------------------------------------------------------
    // イベント
    // -------------------------------------------------------

    /// <summary>
    /// アイテムが追加・更新・削除されたとき発火。
    /// stackがnullのとき削除を表す。
    /// </summary>
    public event Action<Vector2Int, ItemStackModel> OnItemChanged;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------

    public InventoryModel(int width, int height)
    {
        _width = width;
        _height = height;
        _cells = new Vector2Int?[width, height];
        _items = new Dictionary<Vector2Int, ItemStackModel>();
    }

    // -------------------------------------------------------
    // 追加：自動配置
    // -------------------------------------------------------

    /// <summary>
    /// アイテムを自動配置する。
    /// 1. 同じアイテムのスタックに追加（左上優先・maxStackを超えない）
    /// 2. 空きスペースに新規配置（左上優先）
    /// 追加できなかった分をItemStackModelで返す。全部入ったらnull。
    /// </summary>
    public ItemStackModel TryAddAuto(ItemStackModel incoming)
    {
        if (incoming == null || incoming.Amount <= 0) return null;

        int remaining = incoming.Amount;

        // 1. 既存の同種アイテムスタックに補充
        List<(Vector2Int origin, ItemStackModel stack)> sameStacks =
            FindSameItemStacks(incoming.Item);

        foreach ((Vector2Int origin, ItemStackModel stack) in sameStacks)
        {
            if (remaining <= 0) break;
            if (stack.IsFull) continue;

            int space = stack.Item.MaxStack - stack.Amount;
            int toAdd = Mathf.Min(space, remaining);
            stack.Add(toAdd);
            remaining -= toAdd;
            OnItemChanged?.Invoke(origin, stack);
        }

        // 2. 空きスペースに新規配置
        while (remaining > 0)
        {
            Vector2Int? freeOrigin = FindFreeOrigin(incoming.Item.Size);
            if (freeOrigin == null) break;

            int toPlace = Mathf.Min(incoming.Item.MaxStack, remaining);
            ItemStackModel newStack = new ItemStackModel(incoming.Item, toPlace);
            PlaceItem(freeOrigin.Value, newStack);
            remaining -= toPlace;
        }

        if (remaining <= 0) return null;
        return new ItemStackModel(incoming.Item, remaining);
    }

    // -------------------------------------------------------
    // 追加：位置指定
    // -------------------------------------------------------

    /// <summary>
    /// 指定座標にアイテムを配置する。
    /// - 同じアイテムが既にあれば補充し、余ったらItemStackModelで返す
    /// - 十分な空きスペースがあれば配置し、nullを返す
    /// - 不正な座標・スペース不足の場合はincomingをそのまま返す
    /// </summary>
    public ItemStackModel TryAddAt(ItemStackModel incoming, Vector2Int origin)
    {
        if (incoming == null || incoming.Amount <= 0) return null;
        if (!IsInBounds(origin, incoming.Item.Size)) return incoming;

        // 指定範囲内にある既存アイテムを収集
        List<Vector2Int> occupiedOrigins = GetOccupiedOriginsInRect(origin, incoming.Item.Size);

        // 同種アイテムのみの場合 → 補充
        if (occupiedOrigins.Count == 1)
        {
            ItemStackModel existing = _items[occupiedOrigins[0]];
            if (existing.Item.IsSameItem(incoming.Item))
            {
                int space = existing.Item.MaxStack - existing.Amount;
                int toAdd = Mathf.Min(space, incoming.Amount);
                existing.Add(toAdd);
                OnItemChanged?.Invoke(occupiedOrigins[0], existing);

                int leftover = incoming.Amount - toAdd;
                return leftover > 0 ? new ItemStackModel(incoming.Item, leftover) : null;
            }
        }

        // 範囲が完全に空いている場合 → 新規配置
        if (occupiedOrigins.Count == 0 && CanPlace(origin, incoming.Item.Size))
        {
            PlaceItem(origin, incoming);
            return null;
        }

        // それ以外（異種アイテムが混在・スペース不足）→ 失敗
        return incoming;
    }

    // -------------------------------------------------------
    // 入れ替え（同インベントリ内）
    // -------------------------------------------------------

    /// <summary>
    /// 同インベントリ内の2つのアイテムを入れ替える。
    /// どちらか一方でもスペースが足りなければキャンセルしfalseを返す。
    /// </summary>
    public bool TrySwap(Vector2Int originA, Vector2Int originB)
    {
        return TrySwapBetween(this, originA, this, originB);
    }

    // -------------------------------------------------------
    // 入れ替え（別インベントリ間）
    // -------------------------------------------------------

    /// <summary>
    /// 別インベントリ間でアイテムを入れ替える。
    /// どちらか一方でもスペースが足りなければキャンセルしfalseを返す。
    /// </summary>
    public static bool TrySwapBetween(
        InventoryModel invA, Vector2Int originA,
        InventoryModel invB, Vector2Int originB)
    {
        ItemStackModel stackA = invA.GetAt(originA);
        ItemStackModel stackB = invB.GetAt(originB);

        // どちらも存在しない場合は何もしない
        if (stackA == null && stackB == null) return false;

        // 片方がnullの場合は単純な移動として処理
        if (stackA == null)
        {
            if (!invA.CanPlace(originA, stackB.Size)) return false;
            invB.RemoveItem(originB);
            invA.PlaceItem(originA, stackB);
            return true;
        }
        if (stackB == null)
        {
            if (!invB.CanPlace(originB, stackA.Size)) return false;
            invA.RemoveItem(originA);
            invB.PlaceItem(originB, stackA);
            return true;
        }

        // 両方存在する場合：互いのスペースを確認してから入れ替え
        // AをBの位置に置けるか（Bを除いた状態で）
        bool aFitsInB = invB.CanPlaceExcluding(originB, stackA.Size, originB);
        // BをAの位置に置けるか（Aを除いた状態で）
        bool bFitsInA = invA.CanPlaceExcluding(originA, stackB.Size, originA);

        if (!aFitsInB || !bFitsInA) return false;

        invA.RemoveItem(originA);
        invB.RemoveItem(originB);
        invA.PlaceItem(originA, stackB);
        invB.PlaceItem(originB, stackA);
        return true;
    }

    // -------------------------------------------------------
    // 削除（UI操作起点：座標指定）
    // -------------------------------------------------------

    /// <summary>指定座標のアイテムを全て削除し、削除したItemStackModelを返す。なければnull。</summary>
    public ItemStackModel RemoveAt(Vector2Int origin)
    {
        if (!_items.ContainsKey(origin)) return null;
        ItemStackModel stack = _items[origin];
        RemoveItem(origin);
        return stack;
    }

    /// <summary>
    /// 指定座標のアイテムを指定個数削除する。
    /// amountがスタック数以上なら全削除。
    /// 削除後に残ったスタックがある場合はそのままインベントリに残る。
    /// 削除した分をItemStackModelで返す。
    /// </summary>
    public ItemStackModel RemoveAt(Vector2Int origin, int amount)
    {
        if (!_items.ContainsKey(origin)) return null;
        if (amount <= 0) return null;

        ItemStackModel stack = _items[origin];
        int toRemove = Mathf.Min(amount, stack.Amount);

        if (toRemove >= stack.Amount)
        {
            RemoveItem(origin);
            return new ItemStackModel(stack.Item, toRemove);
        }

        stack.Remove(toRemove);
        OnItemChanged?.Invoke(origin, stack);
        return new ItemStackModel(stack.Item, toRemove);
    }

    // -------------------------------------------------------
    // 消費（ゲームロジック起点：アイテム種別指定）
    // -------------------------------------------------------

    /// <summary>
    /// 指定したItemDataのアイテムをamount個消費する。
    /// 左上優先でスタックを消費していき、消費できた個数を返す。
    /// amountに満たない場合は消費できた分だけ消費する。
    /// </summary>
    public int Consume(ItemData data, int amount)
    {
        if (data == null || amount <= 0) return 0;

        // 同種アイテムのスタックを左上優先で取得
        // FindSameItemStacksはItemModelで検索するため、Dataで比較できるよう一時的にスタックを直接探す
        List<(Vector2Int origin, ItemStackModel stack)> targets = FindSameItemStacksByData(data);

        int remaining = amount;
        foreach ((Vector2Int origin, ItemStackModel stack) in targets)
        {
            if (remaining <= 0) break;

            int toConsume = Mathf.Min(stack.Amount, remaining);
            remaining -= toConsume;

            if (toConsume >= stack.Amount)
            {
                RemoveItem(origin);
            }
            else
            {
                stack.Remove(toConsume);
                OnItemChanged?.Invoke(origin, stack);
            }
        }

        return amount - remaining;
    }

    // -------------------------------------------------------
    // クエリ
    // -------------------------------------------------------

    /// <summary>指定座標にあるItemStackModelを返す。なければnull。</summary>
    public ItemStackModel GetAt(Vector2Int origin)
    {
        _items.TryGetValue(origin, out ItemStackModel stack);
        return stack;
    }

    /// <summary>
    /// 指定座標を左上としてsizeマス配置可能か。
    /// 範囲内全マスが空いている場合にtrueを返す。
    /// </summary>
    public bool CanPlace(Vector2Int origin, Vector2Int size)
    {
        if (!IsInBounds(origin, size)) return false;

        for (int x = origin.x; x < origin.x + size.x; x++)
        {
            for (int y = origin.y; y < origin.y + size.y; y++)
            {
                if (_cells[x, y] != null) return false;
            }
        }
        return true;
    }

    /// <summary>sizeが収まる空きスペースが存在するか（配置はしない、判定のみ）</summary>
    public bool CanFitAnywhere(Vector2Int size)
    {
        for (int y = 0; y <= _height - size.y; y++)
        {
            for (int x = 0; x <= _width - size.x; x++)
            {
                if (CanPlace(new Vector2Int(x, y), size)) return true;
            }
        }
        return false;
    }

    // -------------------------------------------------------
    // 内部ヘルパー
    // -------------------------------------------------------

    /// <summary>
    /// excludeOriginのアイテムを除外した状態で配置可能かチェックする。
    /// 入れ替え時のスペース判定に使用。
    /// </summary>
    private bool CanPlaceExcluding(Vector2Int origin, Vector2Int size, Vector2Int excludeOrigin)
    {
        if (!IsInBounds(origin, size)) return false;

        for (int x = origin.x; x < origin.x + size.x; x++)
        {
            for (int y = origin.y; y < origin.y + size.y; y++)
            {
                Vector2Int? occupant = _cells[x, y];
                if (occupant == null) continue;
                if (occupant.Value == excludeOrigin) continue; // 除外対象
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 指定矩形内に存在するアイテムの左上座標リストを返す（重複なし）。
    /// </summary>
    private List<Vector2Int> GetOccupiedOriginsInRect(Vector2Int origin, Vector2Int size)
    {
        List<Vector2Int> origins = new List<Vector2Int>();
        for (int x = origin.x; x < origin.x + size.x; x++)
        {
            for (int y = origin.y; y < origin.y + size.y; y++)
            {
                if (x < 0 || x >= _width || y < 0 || y >= _height) continue;
                Vector2Int? occupant = _cells[x, y];
                if (occupant == null) continue;
                if (!origins.Contains(occupant.Value)) origins.Add(occupant.Value);
            }
        }
        return origins;
    }

    /// <summary>
    /// 同じItemModelを持つスタックを左上優先（y昇順→x昇順）で返す。
    /// </summary>
    private List<(Vector2Int, ItemStackModel)> FindSameItemStacks(ItemModel item)
    {
        List<(Vector2Int, ItemStackModel)> result = new List<(Vector2Int, ItemStackModel)>();
        foreach (KeyValuePair<Vector2Int, ItemStackModel> kv in _items)
        {
            if (kv.Value.Item.IsSameItem(item))
                result.Add((kv.Key, kv.Value));
        }
        // 左上優先：y昇順→x昇順でソート
        result.Sort((a, b) =>
        {
            if (a.Item1.y != b.Item1.y) return a.Item1.y.CompareTo(b.Item1.y);
            return a.Item1.x.CompareTo(b.Item1.x);
        });
        return result;
    }

    /// <summary>
    /// ItemDataで同種検索（Consume用）。左上優先ソート済み。
    /// </summary>
    private List<(Vector2Int, ItemStackModel)> FindSameItemStacksByData(ItemData data)
    {
        List<(Vector2Int, ItemStackModel)> result = new List<(Vector2Int, ItemStackModel)>();
        foreach (KeyValuePair<Vector2Int, ItemStackModel> kv in _items)
        {
            if (kv.Value.Item.Data == data)
                result.Add((kv.Key, kv.Value));
        }
        result.Sort((a, b) =>
        {
            if (a.Item1.y != b.Item1.y) return a.Item1.y.CompareTo(b.Item1.y);
            return a.Item1.x.CompareTo(b.Item1.x);
        });
        return result;
    }

    /// <summary>
    /// sizeが収まる空き座標を左上優先（y昇順→x昇順）で探す。
    /// 見つからなければnull。
    /// </summary>
    private Vector2Int? FindFreeOrigin(Vector2Int size)
    {
        for (int y = 0; y <= _height - size.y; y++)
        {
            for (int x = 0; x <= _width - size.x; x++)
            {
                Vector2Int candidate = new Vector2Int(x, y);
                if (CanPlace(candidate, size)) return candidate;
            }
        }
        return null;
    }

    /// <summary>座標とサイズがグリッド範囲内に収まるか</summary>
    private bool IsInBounds(Vector2Int origin, Vector2Int size)
    {
        return origin.x >= 0 && origin.y >= 0
            && origin.x + size.x <= _width
            && origin.y + size.y <= _height;
    }

    /// <summary>アイテムをグリッドに配置しイベントを発火する</summary>
    private void PlaceItem(Vector2Int origin, ItemStackModel stack)
    {
        _items[origin] = stack;
        Vector2Int size = stack.Size;
        for (int x = origin.x; x < origin.x + size.x; x++)
        {
            for (int y = origin.y; y < origin.y + size.y; y++)
            {
                _cells[x, y] = origin;
            }
        }
        OnItemChanged?.Invoke(origin, stack);
    }

    /// <summary>アイテムをグリッドから削除しイベントを発火する</summary>
    private void RemoveItem(Vector2Int origin)
    {
        if (!_items.TryGetValue(origin, out ItemStackModel stack)) return;

        Vector2Int size = stack.Size;
        for (int x = origin.x; x < origin.x + size.x; x++)
        {
            for (int y = origin.y; y < origin.y + size.y; y++)
            {
                _cells[x, y] = null;
            }
        }
        _items.Remove(origin);
        OnItemChanged?.Invoke(origin, null);
    }
}