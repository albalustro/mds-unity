using System;
using System.Collections.Generic;
using System.Linq;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;
using UnityEngine;

namespace MDS.Validators
{
    public class ValidationRulesGroup : IValidatable
    {

        public ValidationRule[] rules;

        public OperationLogic OperationLogic { get; set; }

        public bool EnableValidationIgnoreRuleIfTargetIsDisabled { get; set; }
        public bool ValidationIgnoreRuleIfTargetIsDisabled { get; set; }
        public bool AlwaysReadyToValidate { get; set; }
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

            if(AlwaysReadyToValidate)
                return true;

            bool ret = false;

            ValidationRule[] list = rules;
            if(EnableValidationIgnoreRuleIfTargetIsDisabled) // vai dar pau se tiver ValidationRulesGroup dentro de ValidationRulesGroup
                list = list.Where(l => l.ValidatableObject.GetGameObject().activeSelf == true).ToArray();

            switch(OperationLogic)
            {
                case OperationLogic.AND:
                    ret = list.All(s => s.ValidatableObject.ReadyToValidate());
                    break;
                case OperationLogic.OR:
                    ret = list.Any(s => s.ValidatableObject.ReadyToValidate());
                    break;
            }

            return ret;
        }

        public bool Validate(string dummy)
        {
            bool ret = false;

            ValidationRule[] list = rules;
            if(ValidationIgnoreRuleIfTargetIsDisabled) // vai dar pau se tiver ValidationRulesGroup dentro de ValidationRulesGroup
                list = list.Where(l => l.ValidatableObject.GetGameObject().activeSelf == true).ToArray();

            switch(OperationLogic)
            {
                case OperationLogic.AND:
                    ret = list.All(s => s.ValidatableObject.Validate(s.CorrectAnswer));
                    break;
                case OperationLogic.OR:
                    ret = list.Any(s => s.ValidatableObject.Validate(s.CorrectAnswer));
                    break;
            }

            return ret;
        }

        public GameObject GetGameObject()
        {
            Debug.LogError("ValidationRulesGroup dentro de ValidationRulesGroup");
            return null;
        }

        #endregion

    }
}
