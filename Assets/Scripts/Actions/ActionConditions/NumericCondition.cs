using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;
using MDS.Actions.DialogConditions;
using MDS.Validators.Interfaces;

namespace MDS.Actions.DialogConditions
{
    public class NumericCondition : ConditionedActionBase
    {
        [InspectorCategory("Condition")]
        [SerializeField, InspectorTooltip("Group cujo valor numérico será comparado")]
        private IValidatable _validatable;

        [InspectorCategory("Condition")]
        [SerializeField]
        private MathValidador.Operation _operation;

        [InspectorCategory("Condition")]
        [SerializeField]
        private int _value;

        public override bool IsConditionSatisfied()
        {
            if(_validatable.GetNumericValue().HasValue == false)
            {
                _validatable.GetGameObject().GetComponent<MDSBehaviour>().LogError("NumericCondition tentando avaliar valor numerico de um validatable que nao possui valor numerico");
                return false;
            }

            int groupValue = _validatable.GetNumericValue().Value;

            switch(_operation)
            {
                case MathValidador.Operation.Greater:
                    return groupValue > _value;

                case MathValidador.Operation.GreaterOrEqual:
                    return groupValue >= _value;

                case MathValidador.Operation.Equal:
                    return groupValue == _value;

                case MathValidador.Operation.Lesser:
                    return groupValue < _value;

                case MathValidador.Operation.LesserOrEqual:
                    return groupValue <= _value;

                default:
                    break;
            }

            return false;
        }
    }
}
