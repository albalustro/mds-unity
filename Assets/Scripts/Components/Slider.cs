using System.Collections;
using System.Collections.Generic;
using MDS.Validators.Interfaces;
using UnityEngine;

public class Slider : MDSBehaviour {

    [SerializeField]
    private IValidatableGroup _group;

    [SerializeField]
    private string _label;

    [SerializeField]
    private int _steps;
    private int _currentStep;

    private Transform _sliderPointer;
    private Transform _initialPositionReference;
    private Transform _finalPositionReference;

    private Vector3 _vStep;


    protected override void Awake()
    {
        base.Awake();

        
        _sliderPointer = transform.Find("Sprite Indicador");
        _initialPositionReference = transform.Find("Initial Position");
        _finalPositionReference = transform.Find("Final Position");


        _vStep = (_finalPositionReference.localPosition - _initialPositionReference.localPosition) / _steps;
        _currentStep = 0;


        if(_sliderPointer == null || _initialPositionReference == null || _finalPositionReference == null)
            LogError("Um dos elementos do slider não foi encontrado. Os nomes devem ser mantidos sem alterações");
    }


    public void IncrementStep()
    {
        _currentStep++;
        if(_currentStep > _steps)
            _currentStep = _steps;

        RefreshPointerPosition();
    }


    public void Update()
    {
        if(_group == null) return;

        _currentStep = _group.Count(_label);

        RefreshPointerPosition();
    }


    private void RefreshPointerPosition()
    {
        _sliderPointer.localPosition = _initialPositionReference.localPosition + _vStep * _currentStep;
    }
}
