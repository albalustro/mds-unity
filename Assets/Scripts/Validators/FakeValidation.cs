using System;
using System.Collections;
using System.Collections.Generic;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;
using UnityEngine;

public class FakeValidation : MDSBehaviour, IValidator
{
    public bool ReadyToValidate()
    {
        return true;
    }

    public ValidatorResult Validate()
    {
        return ValidatorResult.Victory;
    }
}
