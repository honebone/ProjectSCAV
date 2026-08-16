using UnityEngine;

/// <summary>
/// GunStatへの補正1件分（対象ステータス・実数or割合・量）
/// Implant/Tool/Buffなど、装備中パッシブ効果から共通で使う
/// 適用/除去はExtensions.ApplyTo/RemoveFromを使う（LoadoutModel.PassiveModifiersへPull型で登録される）
/// </summary>
[System.Serializable]
public struct GunStatModifier
{
    [SerializeField] private GunStatType _stat;
    [SerializeField] private ModifierKind _kind;
    [SerializeField] private float _amount;

    public GunStatType Stat => _stat;
    public ModifierKind Kind => _kind;
    public float Amount => _amount;
}

/// <summary>
/// PjtlStatへの補正1件分（対象ステータス・実数or割合・量）
/// Implant/Tool/Buffなど、装備中パッシブ効果から共通で使う
/// 適用/除去はExtensions.ApplyTo/RemoveFromを使う（LoadoutModel.PassiveModifiersへPull型で登録される）
/// </summary>
[System.Serializable]
public struct PjtlStatModifier
{
    [SerializeField] private PjtlStatType _stat;
    [SerializeField] private ModifierKind _kind;
    [SerializeField] private float _amount;

    public PjtlStatType Stat => _stat;
    public ModifierKind Kind => _kind;
    public float Amount => _amount;
}
