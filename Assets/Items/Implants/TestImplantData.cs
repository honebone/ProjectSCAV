using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ImplantData/TestImplant")]

public class TestImplantData : ImplantData
{
    [SerializeField, Header("静止している間、銃のステータスへ加える補正")] private GunStatModifier[] _gunStatModifiersOnStanding;
    public IReadOnlyList<GunStatModifier> GunStatModifiersOnStanding => _gunStatModifiersOnStanding;

    public override ItemModel CreateModel()
    {
        return new TestImplantModel(this);
    }
}
