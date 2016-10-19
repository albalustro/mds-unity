
using MDS.Validators.Interfaces;
using FullInspector;
using MDS.Validators.Enum;
using System.Linq;

namespace MDS.Validators
{
    public class ValidationRule
    {
        [InspectorOrder(0)]
        public IValidatable ValidatableObject;

        [InspectorOrder(1)]
        [InspectorShowIf("IsAGroup")]
        public bool OverrideGroupParameters;

        [InspectorOrder(2)]
        [InspectorShowIf("OverrideGroupParameters")]
        public OperationLogic OperationLogic { get; set; }

        [InspectorOrder(3)]
        [InspectorShowIf("OverrideGroupParameters")]
        public bool AcceptEmptyAsCorrectAnswer { get; set; }

        [InspectorOrder(4)]
        [InspectorShowIf("OverrideGroupParameters")]
        public int? SpecificAmount { get; set; }

        [InspectorOrder(5)]
        [InspectorHideIf("IsRuleGroup")]
        public string CorrectAnswer;


        public bool IsSatisfied()
        {

            if(OverrideGroupParameters && IsAGroup())
            {
                IValidatableGroup group = ValidatableObject as IValidatableGroup;
                group.OperationLogic = OperationLogic;
                group.SpecificAmount = SpecificAmount;
                group.AcceptEmptyAsCorrectAnswer = AcceptEmptyAsCorrectAnswer;
            }

            if(IsRuleGroup())
            {
                ValidationRulesGroup ruleGroup = ValidatableObject as ValidationRulesGroup;
                foreach(ValidationRule rule in ruleGroup.rules.Where(r => r.IsAGroup() && r.OverrideGroupParameters))
                {
                    IValidatableGroup group = rule.ValidatableObject as IValidatableGroup;
                    group.OperationLogic = rule.OperationLogic;
                    group.SpecificAmount = rule.SpecificAmount;
                    group.AcceptEmptyAsCorrectAnswer = rule.AcceptEmptyAsCorrectAnswer;
                }

            }

            if(!ValidatableObject.ReadyToValidate())
                return false;

            return ValidatableObject.Validate(CorrectAnswer);
        }

        private bool IsAGroup()
        {            
            if(ValidatableObject == null) return false;

            return ValidatableObject is IValidatableGroup;
        }

        private bool IsRuleGroup()
        {
            if(ValidatableObject == null) return false;

            return ValidatableObject is ValidationRulesGroup;
        }
    }
}
