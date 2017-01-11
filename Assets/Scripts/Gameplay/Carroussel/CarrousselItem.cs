using UnityEngine;
using System.Collections;
using FullInspector;
using MDS.Validators.Interfaces;
using System;

public class CarrousselItem : MDSBehaviour, IValidatable
{

    [SerializeField, InspectorTooltip("Caso selecione o primeiro valor, esse será usado como indice e, a partir dele, os demais serão incrementados")]
    private string firstValue;

    [SerializeField, InspectorTooltip("Use esse valor para indicar que o Item não é validável")]
    private string invalidValue;

 

    [SerializeField, InspectorTooltip("Multiplicador usado ao buscar o valor numerico")]
    private int? _numericMultiplier;

    private int _currentItemIndex;
    private int _maxItemIndex;

    private Vector3 _startPosition;

    

    private CarrousselWindow _parentWindow;

    public void Initialize(CarrousselWindow carrousselWindow)
    {
        _parentWindow = carrousselWindow;
    }

    public void Start()
    {
        _currentItemIndex = 0;
        _startPosition = transform.localPosition; 
        _maxItemIndex = transform.childCount-1;
    }

    public void MoveNext()
    {
        MoveNext(.25f);
    }

    internal void MoveNext(float _transitionTime)
    {
        _currentItemIndex++;
        if(_currentItemIndex > _maxItemIndex)
            _currentItemIndex = 0;

        Vector3 destination = _startPosition;
        destination.y = -_currentItemIndex;

        LeanTween.moveLocal(base.gameObject, destination, _transitionTime).setEase(LeanTweenType.easeInOutCubic);
    }

    public string GetCurrentValue()
    {
        return transform.GetChild(_currentItemIndex).GetComponent<CarrousselItemValue>().value;
    }

    public void Freeze()
    {
        _parentWindow.Freeze();
    }

    [InspectorButton]
    public void SetupChildrenValue()
    {
        bool isNumber = true;

        if(string.IsNullOrEmpty(firstValue))
            firstValue = "0";


        int fv;

        isNumber = int.TryParse(firstValue, out fv);

        for(int i = 0; i < transform.childCount; i++)
        {
            CarrousselItemValue v = transform.GetChild(i).GetComponent<CarrousselItemValue>();

            if(v == null)
                v = transform.GetChild(i).gameObject.AddComponent<CarrousselItemValue>();

            if(isNumber)
                v.value = (fv + i).ToString();
            else
                v.value = ((char)(firstValue[0] + i)).ToString();
        }
    }

    #region IValidatable

    public bool ReadyToValidate()
    {
        if(string.IsNullOrEmpty(invalidValue))
            return true;
        else
            return !GetCurrentValue().Equals(invalidValue);
    }

    public bool Validate(string acceptableAnswer)
    {
        return GetCurrentValue().Equals(acceptableAnswer);
    }

    public int? GetNumericValue()
    {
        int result;
        if(ReadyToValidate() && int.TryParse(GetCurrentValue(), out result))
        {
            if(_numericMultiplier.HasValue == true)
                result *= _numericMultiplier.Value;
            return result;
        }
        return null;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion
}
