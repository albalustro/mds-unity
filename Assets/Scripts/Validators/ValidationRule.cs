
using MDS.Validators.Interfaces;
using FullInspector;
using MDS.Validators.Enum;
using System.Linq;
using MDS.Gameplay.DragDrop;

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
        [InspectorShowIf("OverrideGroupParameters")]
        public bool EnableValidationOnlyIfSpecifcAmount;

        [InspectorOrder(6)]
		[InspectorShowIf("OverrideGroupParameters")]
		public bool isGeneric { get; set; }

        [InspectorOrder(7)]
        [InspectorHideIf("IsRuleGroup")]
        public string CorrectAnswer;

        public bool ReadyToValidate()
        {
            SetupOverrideParams();

            if(!ValidatableObject.ReadyToValidate())
                return false;

            return true;

        }

        private void SetupOverrideParams()
        {
            if(OverrideGroupParameters && IsAGroup())
            {
                IValidatableGroup group = ValidatableObject as IValidatableGroup;
                group.OperationLogic = OperationLogic;
                group.SpecificAmount = SpecificAmount;
                // gambis feita as pressas... close your eyes..
                ValidatableDropGroupArea gg = group as ValidatableDropGroupArea;
                if(gg != null)
                    gg.SetEnableValidationOnlyIfSpecifcAmount(EnableValidationOnlyIfSpecifcAmount);
                // fim da gambis
                group.AcceptEmptyAsCorrectAnswer = AcceptEmptyAsCorrectAnswer;
                group.isGeneric = isGeneric;
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
                    // gambis feita as pressas... close your eyes..
                    ValidatableDropGroupArea gg = group as ValidatableDropGroupArea;
                    if(gg != null)
                        gg.SetEnableValidationOnlyIfSpecifcAmount(EnableValidationOnlyIfSpecifcAmount);
                    // fim da gambis
                    group.isGeneric = rule.isGeneric;
                }

            }
        }

        public bool IsSatisfied()
        {

            if(!ReadyToValidate()) return false;
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
