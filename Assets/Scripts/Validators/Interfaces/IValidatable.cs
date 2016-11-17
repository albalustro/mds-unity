
using FullInspector;

namespace MDS.Validators.Interfaces
{
    public interface IValidatable
    {

        bool ReadyToValidate();
        bool Validate(string acceptableAnswer);
        int? GetNumericValue(); 
    }
}