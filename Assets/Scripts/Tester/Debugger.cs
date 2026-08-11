using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Debugger : MonoBehaviour
{
    [SerializeField] float emergencyInterval;

    [SerializeField] EntityStatsData _entityStatsData;
    [SerializeField] EntityPresenter _presenter;
    [SerializeField] MapSpawner _mapSpawner;
    [SerializeField] NavGraphScanner _scanner;
    [SerializeField] UIManager _uiManager;
    [SerializeField] AreaManager _areaManager;

    [SerializeField] EntityStatsData _enemyData;
    [SerializeField] EntityPresenter _enemyPresenter;

    [SerializeField] AreaData areaData;
    [SerializeField] List<GunData> gunData;
    [SerializeField] List<ItemData> itemData;
    [SerializeField] List<int> itemAmount;

    [Header("パッシブ効果・作用(EffectAction)の検証用")]
    [SerializeField, Header("Alpha3キーでプレイヤーにダメージ+バフを同時付与するEffectActionを発火")] private BuffApplication _testBuff;
    [SerializeField, Header("Alpha3キーで与えるダメージ量")] private float _testDamage = 10f;

    // Start is called before the first frame update
    async void Start()
    {
        // �f�[�^�x�[�X�̃��[�h������҂�
        // �iBootstrap�V�[�����o�R�����{�V�[���𒼐ڍĐ������ꍇ�������ŒS�ۂ����j
        await GameBootstrapper.WaitForReadyAsync();

        _areaManager.Init(areaData);

        _presenter.Init(_entityStatsData, _scanner.Pathfinder);
        _enemyPresenter.Init(_enemyData, _scanner.Pathfinder);  
        if (_presenter.Model is PlayerModel model)
        {
            for (int i = 0; i < gunData.Count; i++)
            {
                ItemStackModel itemStack = new ItemStackModel(gunData[i].CreateModel(), 1);
                model.Loadout.TryEquip(i, itemStack, model);
            }

            for (int i = 0; i < itemData.Count; i++)
            {
                ItemStackModel itemStack = new ItemStackModel(itemData[i].CreateModel(), itemAmount[i]);
                model.Inventory.TryAddAuto(itemStack);
            }

            _uiManager.Init(model.Inventory);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(Emergency());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_presenter.Model is PlayerModel model)
            {
                for (int i = 0; i < itemData.Count; i++)
                {
                    ItemStackModel itemStack = new ItemStackModel(itemData[i].CreateModel(), itemAmount[i]);
                    model.Inventory.TryAddAuto(itemStack);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // ダメージとバフ付与を同時に持つEffectActionを1件発火し、複合効果が解決されることを確認する
            if (_presenter.Model is PlayerModel model)
            {
                List<BuffApplication> buffs = new List<BuffApplication> { _testBuff };
                model.Resolve(new EffectAction(model, damageAmount: _testDamage, damageTarget: DamageTarget.HPAndArmor, buffs: buffs));
                DevLog.Log($"[Debugger] EffectActionテスト発火 damage:{_testDamage} buff:{(_testBuff.Buff != null ? _testBuff.Buff.name : "なし")}");
            }
        }
    }

    IEnumerator Emergency()
    {
        Props[] props = FindObjectsOfType<Props>();
        foreach (var prop in props)
        {
            prop.Test();
        }

        yield return new WaitForSeconds(emergencyInterval);

        foreach (var prop in props)
        {
            prop.Test2();
        }
    }
}
