using UnityEngine;
using System.Collections;
using MDS.Validators.Interfaces;
using UnityEngine.SceneManagement;

public class BarGraph : MDSBehaviour {

    #region Fields & Properties

    [SerializeField]
    private IValidatableGroup _group;

    [SerializeField]
    private string _label;

    private SpriteRenderer[] _barPositions;

    private bool _logError1 = true;

    #endregion

    #region Unity Methods

    protected override void Awake()
    {
        base.Awake();

        _barPositions = GetComponentsInChildren<SpriteRenderer>();
    }

    public void Start()
    {
        TurnOffBarGraph();
    }

    public void Update()
    {
        int amount = _group.Count(_label);

        if(amount > _barPositions.Length)
        {
            if(_logError1)
            {
                LogError("BarGraph com menos posicoes que o necessário.");
                _logError1 = false;
            }
            TurnOnBarGraph();
            return;
        }

        TurnOnBarGraph(amount);
    }

    #endregion

    #region Methods

    private void TurnOffBarGraph()
    {
        foreach(var bar in _barPositions)
        {
            bar.enabled = false;
        }
    }

    private void TurnOnBarGraph()
    {
        foreach(var bar in _barPositions)
        {
            bar.enabled = true;
        }
    }

    private void TurnOnBarGraph(int amount)
    {
        for(int i = 0; i < _barPositions.Length; i++)
        {
            if(amount >= (i + 1))
                _barPositions[i].enabled = true;
            else
                _barPositions[i].enabled = false;
        }
    }

    #endregion


}
