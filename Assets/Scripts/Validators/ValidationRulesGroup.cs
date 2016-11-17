using System;
using System.Linq;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;

namespace MDS.Validators
{
    public class ValidationRulesGroup : IValidatable
    {

        public ValidationRule[] rules;

        public OperationLogic OperationLogic { get; set; }

        public int? GetNumericValue()
        {
            bool hasResult = false;
            int result = 0;
            int? temp;
            foreach(var r in rules)
            {
                temp = r.ValidatableObject.GetNumericValue();
                if(temp.HasValue)
                {
                    result += temp.Value;
                    hasResult = true;
                }

            }
            if(hasResult)
                return result;
            return null;
        }


        #region IValidatable

        public bool ReadyToValidate()
        {
            bool ret = false;

            switch(OperationLogic)
            {
                case OperationLogic.AND:
                    ret = rules.All(s => s.ValidatableObject.ReadyToValidate());
                    break;
                case OperationLogic.OR:
                    ret = rules.Any(s => s.ValidatableObject.ReadyToValidate());
                    break;
            }

            return ret;
        }

        public bool Validate(string dummy)
        {
            bool ret = false;

            switch(OperationLogic)
            {
                case OperationLogic.AND:
                    ret = rules.All(s => s.ValidatableObject.Validate(s.CorrectAnswer));
                    break;
                case OperationLogic.OR:
                    ret = rules.Any(s => s.ValidatableObject.Validate(s.CorrectAnswer));
                    break;
            }

            return ret;
        }

        #endregion

    }
}
