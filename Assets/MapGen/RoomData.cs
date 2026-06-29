using UnityEngine;

/// <summary>
/// 部屋1種類分のデータ
/// 通路の組み合わせ(16パターン)とプレハブ、出現ウェイトを保持する
/// </summary>
[CreateAssetMenu(menuName = "MapGen/RoomData")]
public class RoomData : ScriptableObject
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private bool _hasNorth;
    [SerializeField] private bool _hasSouth;
    [SerializeField] private bool _hasEast;
    [SerializeField] private bool _hasWest;

    /// <summary>出現頻度の重み。大きいほど選ばれやすい</summary>
    [SerializeField] private float _weight = 1f;

    public GameObject Prefab => _prefab;
    public bool HasNorth => _hasNorth;
    public bool HasSouth => _hasSouth;
    public bool HasEast => _hasEast;
    public bool HasWest => _hasWest;
    public float Weight => _weight;

    /// <summary>この部屋が指定方向に通路を持つか返す</summary>
    public bool HasPassage(Direction dir) => DirectionUtility.GetPassage(this, dir);

    /// <summary>デバッグ表示用の通路パターン文字列 例: [N S E _]</summary>
    public string PassagePattern =>
        $"[{(_hasWest ? "W" : "_")}{(_hasNorth ? "N" : "_")}{(_hasSouth ? "S" : "_")}{(_hasEast ? "E" : "_")}]";
}