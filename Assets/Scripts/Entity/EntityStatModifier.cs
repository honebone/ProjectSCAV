using UnityEngine;

/// <summary>ステータス補正の種類（実数加算か倍率変動か）</summary>
public enum ModifierKind { Flat, Multiplier }

/// <summary>
/// EntityStatへの補正1件分（対象ステータス・実数or割合・量）
/// Implant/Tool/Buffなど、自身のステータスを補正するパッシブ効果から共通で使う
/// 適用/除去はExtensions.ApplyTo/RemoveFromを使う
/// </summary>
[System.Serializable]
public struct EntityStatModifier
{
    [SerializeField] private EntityStatType _stat;
    [SerializeField] private ModifierKind _kind;
    [SerializeField] private float _amount;

    public EntityStatType Stat => _stat;
    public ModifierKind Kind => _kind;
    public float Amount => _amount;
}
