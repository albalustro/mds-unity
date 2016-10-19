using UnityEngine;
using System.Linq;
using MDS.Validators.Interfaces;
using MDS.Validators.Enum;
using FullInspector;

namespace MDS.Gameplay.DragDrop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class ValidatableDropGroupArea : BaseDropGroupArea, IValidatableGroup, IValidatable
    {

        #region IValidatableGroup

        [InspectorMargin(10)]

        public bool Overwritten { get; set; }

        [InspectorHideIf("Overwritten")]
        public OperationLogic OperationLogic { get; set; }

        [InspectorHideIf("Overwritten")]
        public bool AcceptEmptyAsCorrectAnswer { get; set; }

        [InspectorHideIf("Overwritten")]
        public int? SpecificAmount { get; set; }

        #endregion

        #region IValidatable

        public bool ReadyToValidate()
        {
            bool ret = false;

            ret = slots.Any(s => s.ReadyToValidate());

            return ret;
        }

        public bool Validate(string acceptableAnswer)
        {
            bool ret = false;

            DropGroupSlot[] temp = slots;
            if(AcceptEmptyAsCorrectAnswer)
                temp = slots.Where(s => s.draggableReference != null).ToArray();


            switch(OperationLogic)
            {
                case OperationLogic.AND:
                    ret = temp.All(s => s.Validate(acceptableAnswer));
                    break;
                case OperationLogic.OR:
                    ret = temp.Any(s => s.Validate(acceptableAnswer));
                    break;
            }


            if(ret && SpecificAmount.HasValue)
            {
                int qtde = temp.Where(s => s.Validate(acceptableAnswer)).ToList().Count;
                ret = (qtde == SpecificAmount.Value);
            }


            return ret;
        }

        #endregion

    }
}