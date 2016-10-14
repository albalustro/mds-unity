using MDS.Core;

namespace MDS.Validators
{
    public class SingleSelectableValidator : MDSBehaviour, IValidator
    {
        public SelectableGroup selectableGroup;
        public string validChoice;

        public bool ReadyToValidate()
        {
            return selectableGroup.GetSelectable() != null;
        }

        public ValidatorResult Validate()
        {
            Selectable s = selectableGroup.GetSelectable();

            if(s == null)
                return ValidatorResult.NotEnoughParameters;

            if(s.ChoiseTag == validChoice)
                return ValidatorResult.Victory;

            return ValidatorResult.Error;
        }
    }

}