using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MDS.Validators.Interfaces;
using UnityEngine;

public class CarrousselNumericGrouping : MDSBehaviour, IValidatable
{
    private CarrousselItem[] _itens;

    protected override void Awake()
    {
        base.Awake();
        _itens = GetComponentsInChildren<CarrousselItem>();
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public int? GetNumericValue()
    {
        int sum = 0;

        if(_itens.Any(item => item.GetNumericValue().HasValue == false))
            return null;

        for(int i = 0; i < _itens.Length; i++)
        {
            sum += _itens[i].GetNumericValue().Value;
        }

        return sum;
    }

    public bool ReadyToValidate()
    {
        return _itens.All(item => item.ReadyToValidate());
    }

    public bool Validate(string acceptableAnswer)
    {
        int? res = GetNumericValue();

        if(res.HasValue == false)
            return false;
        return res.ToString().Equals(acceptableAnswer);
    }

}
