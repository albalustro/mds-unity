using UnityEngine;
using System.Linq;
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

	public bool m_multiComparison;

	[InspectorShowIf("m_multiComparison")]
	public IValidatable[] m_validatables;



    public bool ReadyToValidate()
    {
		bool ret = false;

		if (m_multiComparison) {
			ret = m_validatables.All (v => v.GetNumericValue ().HasValue);
		} else {
			
			int? firstArg = 0;
			int? secondArg = 0;

			firstArg = _validatableA.GetNumericValue();
			secondArg = _compareTwoElements ? _validatableB.GetNumericValue() : _constant;

			ret = firstArg.HasValue && secondArg.HasValue;
		}

		return (ret);
    }

    public ValidatorResult Validate()
    {

        if(!ReadyToValidate())
            return ValidatorResult.NotEnoughParameters;

        ValidatorResult result = ValidatorResult.Error; ;

		if (m_multiComparison) {

			int quant = 0;

			foreach (var item in m_validatables) {
				quant = m_validatables.Where (v => v.GetNumericValue() == item.GetNumericValue()).Count();
				if (quant > 1) {
					result = ValidatorResult.Error;
					return result;
				}
			}

			result = ValidatorResult.Victory;

		} else {
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
