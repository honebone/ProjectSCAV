using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Debugger : MonoBehaviour
{
    [SerializeField] float emergencyInterval;

    [SerializeField] EntityStatsData _entityStatsData;
    [SerializeField] EntityPresenter _presenter;
    [SerializeField] NavGraphScanner _scanner;

    [SerializeField] EntityStatsData _enemyData;
    [SerializeField] EntityPresenter _enemyPresenter;

    [SerializeField] List<GunData> gunData;


    // Start is called before the first frame update
    void Start()
    {
        _presenter.Init(_entityStatsData, _scanner.Pathfinder);
        _enemyPresenter.Init(_enemyData, _scanner.Pathfinder);  
        if (_presenter.Model is PlayerModel model)
        {
            for (int i = 0; i < gunData.Count; i++)
            {
                ItemStackModel itemStack = new ItemStackModel(gunData[i].CreateModel(), 1);
                model.Loadout.TryEquip(i, itemStack, model);
            }

            //for (int i = 0; i < itemData.Count; i++)
            //{
            //    ItemStackModel itemStack = new ItemStackModel(itemData[i].CreateModel(), itemAmount[i]);
            //    model.Inventory.TryAddAuto(itemStack);
            //}
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(Emergency());
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
