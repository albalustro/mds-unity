using UnityEngine;
using System.Collections;
using MDS.Validators.Interfaces;
using MDS.Validators.Enum;
using System;
using FullInspector;
using System.Collections.Generic;
using MDS.Validators;

public class MathValidador : MDSBehaviour, IValidator
{

    public enum Operation { Greater, GreaterOrEqual, Equal, Lesser, LesserOrEqual}

    public IValidatable _validatableA;

    public Operation _operation;

    public bool _compareTwoElements = false;

    [InspectorShowIf("_compareTwoElements")]
    public IValidatable _validatableB;

    [InspectorShowIf("compareConstante")]
    public int _constant = 0;

    public bool compareConstante() { return !_compareTwoElements; }



    public bool ReadyToValidate()
    {
        int? firstArg = 0;
        int? secondArg = 0;

        firstArg = _validatableA.GetNumericValue();
        secondArg = _compareTwoElements ? _validatableB.GetNumericValue() : _constant;
       
        return (firstArg.HasValue && secondArg.HasValue);
    }

    public ValidatorResult Validate()
    {

        if(!ReadyToValidate())
            return ValidatorResult.NotEnoughParameters;

        ValidatorResult result = ValidatorResult.Error; ;

        int? firstArg = 0;
        int? secondArg = 0;

        firstArg = _validatableA.GetNumericValue();
        secondArg = _compareTwoElements ? _validatableB.GetNumericValue() : _constant;

        switch(_operation)
        {
            case Operation.Greater:
                if(firstArg.Value > secondArg.Value)
                    result = ValidatorResult.Victory;
                break;

            case Operation.GreaterOrEqual:
                if(firstArg.Value >= secondArg.Value)
                    result = ValidatorResult.Victory;
                break;

            case Operation.Equal:
                if(firstArg.Value == secondArg.Value)
                    result = ValidatorResult.Victory;
                break;

            case Operation.Lesser:
                if(firstArg.Value < secondArg.Value)
                    result = ValidatorResult.Victory;
                break;

            case Operation.LesserOrEqual:
                if(firstArg.Value <= secondArg.Value)
                    result = ValidatorResult.Victory;
                break;
        }

        

        return result;
    }


    public bool GetNumericValues(out int? A, out int? B)
    {
        A = _validatableA.GetNumericValue();
        B = _compareTwoElements ? _validatableB.GetNumericValue() : _constant;
        return (A.HasValue && B.HasValue);
    }

    private bool IsAGroup(IValidatable arg)
    {
        if(arg == null) return false;

        return arg is IValidatableGroup;
    }


}
