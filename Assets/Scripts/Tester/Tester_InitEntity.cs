using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester_InitEntity : MonoBehaviour
{
    [SerializeField] EntityStatsData _entityStatsData;
    [SerializeField] EntityPresenter _presenter;
    [SerializeField] NavGraphScanner _scanner;
    // Start is called before the first frame update
    void Start()
    {
        _presenter.Init(_entityStatsData, _scanner.Pathfinder);
    }
}
