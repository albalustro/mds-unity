using System.Collections.Generic;

using MDS.Core;

namespace MDS.Validators
{
    public class MultipleSelectableValidator : MDSBehaviour, IValidator
    {
        public SelectableGroup selectableGroup;
        public List<string> validList;

        public bool ReadyToValidate()
        {
            return selectableGroup.HasAnyoneSelected();
        }

        public ValidatorResult Validate()
        {
            int count = 0;
            List<string> s = selectableGroup.GetSelectables();

            if (s.Count == 0)
                return ValidatorResult.NotEnoughParameters;

            foreach (var item in s)
            {
                if (validList.Contains(item))
                    count++;
            }

            s.Clear();

            if(count == validList.Count)
                return ValidatorResult.Victory;

            return ValidatorResult.Error;
        }
    }

}