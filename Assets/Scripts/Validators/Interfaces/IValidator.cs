using MDS.Validators.Enum;

namespace MDS.Validators.Interfaces
{
    public interface IValidator
    {

        bool ReadyToValidate();
        ValidatorResult Validate();

    }

}