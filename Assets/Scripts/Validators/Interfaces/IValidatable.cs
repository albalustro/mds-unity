
using FullInspector;
using UnityEngine;

namespace MDS.Validators.Interfaces
{
    public interface IValidatable
    {

        bool ReadyToValidate();
        bool Validate(string acceptableAnswer);
        int? GetNumericValue();

        GameObject GetGameObject();
    }
}