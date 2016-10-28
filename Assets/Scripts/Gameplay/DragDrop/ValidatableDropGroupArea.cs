using UnityEngine;
using System.Linq;
using MDS.Validators.Interfaces;
using MDS.Validators.Enum;
using FullInspector;
using System.Collections.Generic;

namespace MDS.Gameplay.DragDrop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class ValidatableDropGroupArea : BaseDropGroupArea, IValidatableGroup, IValidatable
    {
        [ShowInInspector, SerializeField, InspectorTooltip("Selecione para fazer com que o objeto e o slot fiquem bloqueados após um draggable ser solto sobre esse grupo")]
        private bool _freezeAfterDrop;

        [InspectorShowIf("ShowEnableValidation"), SerializeField, InspectorTooltip("Somente se a quantidade especifica de elementos estiver nos slots desse grupo é que ele estará pronto para ser validado. Caso contrário, com apenas um elemento já fica liberado para tentar validar")]
        private bool _enableValidationOnlyIfSpecifcAmount;
        private bool ShowEnableValidation
        {
            get
            {
                return (SpecificAmount.HasValue && !Overwritten);
            }
        }

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

            var readyCount = slots.Where(s => s.ReadyToValidate()).ToList().Count;
            

            if(readyCount > 0)
            {
                if(_enableValidationOnlyIfSpecifcAmount)
                {
                    ret = (readyCount == SpecificAmount);
                }
                else
                {
                    ret = true;
                }
            }
            else
            {
                ret = AcceptEmptyAsCorrectAnswer;
            }

            return ret;
        }

        public bool Validate(string acceptableAnswer)
        {
            bool ret = false;

            List<DropGroupSlot> temp = slots;
            if(AcceptEmptyAsCorrectAnswer)
                temp = slots.Where(s => s.draggableReference != null).ToList();


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

        public override bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
        {
            bool ret = base.SetInSlot(draggable, ref slot);

            if(ret && _freezeAfterDrop)
            {
                Freeze(draggable.gameObject);
                Freeze(slot.gameObject);
            }

            return ret;
        }

        private void Freeze(GameObject go)
        {
            go.GetComponent<Collider2D>().enabled = false;
        }


    }
}