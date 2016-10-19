using System.Linq;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;

namespace MDS.Validators
{
    public class ValidationRulesGroup : IValidatable
    {

        public ValidationRule[] rules;

        public OperationLogic OperationLogic { get; set; }


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
