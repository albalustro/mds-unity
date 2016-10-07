using UnityEngine;
using System.Collections;

namespace MDS.Core
{
    public interface IValidator
    {

        bool ReadyToValidate();
        ValidatorResult Validate();

    }
}