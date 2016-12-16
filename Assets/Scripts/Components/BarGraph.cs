using UnityEngine;
using System.Collections;
using MDS.Validators.Interfaces;
using UnityEngine.SceneManagement;
using System;

public class BarGraph : MDSBehaviour, IValidatable
{

    #region Fields & Properties

    [SerializeField]
    private IValidatableGroup _group;

    [SerializeField]
    private string _label;

    private SpriteRenderer[] _barPositions;

    private bool _logError1 = true;

    private int _amount;

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
        if(_group == null) return;

        _amount = _group.Count(_label);

        if(_amount > _barPositions.Length)
        {
            if(_logError1)
            {
                LogError("BarGraph com menos posicoes que o necessário.");
                _logError1 = false;
            }
            _amount = _barPositions.Length;
            TurnOnBarGraph();
            return;
        }

        TurnOnBarGraph();
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
        for(int i = 0; i < _barPositions.Length; i++)
        {
            if(_amount >= (i + 1))
                _barPositions[i].enabled = true;
            else
                _barPositions[i].enabled = false;
        }
    }

    public void Increment()
    {
        _amount++;
        if(_amount > _barPositions.Length)
            _amount = _barPositions.Length;
        TurnOnBarGraph();
    }
    public void Decrement()
    {
        _amount--;
        if(_amount < 0)
            _amount = 0;
        TurnOnBarGraph();
    }

    #endregion

    #region IValidatable

    public bool ReadyToValidate()
    {
        return true;
    }

    public bool Validate(string acceptableAnswer)
    {
        int value;

        if(int.TryParse(acceptableAnswer, out value))
            return _amount == value;

        return false;
    }

    public int? GetNumericValue()
    {
        return _amount;
    }
    #endregion


}
