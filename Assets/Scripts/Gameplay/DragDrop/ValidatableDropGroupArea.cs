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

      
        [SerializeField]
        private bool _isGeneric;

		[SerializeField, InspectorTooltip("Selecione para fazer com que o objeto sofra um FADE OUT OnDrop")]
		protected bool _fadeAndFreezeOnDrop;

		[InspectorShowIf("_fadeAndFreezeOnDrop")]
		[SerializeField, InspectorTooltip("Tempo, em segundos, para a animação de fade ocorrer")]
		protected float _animationTime;

		[InspectorHideIf("_fadeAndFreezeOnDrop")]
		[SerializeField, InspectorTooltip("Selecione para fazer com que o objeto e o slot fiquem bloqueados após um draggable ser solto sobre esse grupo")]
		protected bool _freezeAfterDrop;

		[InspectorShowIf("ShowEnableValidation"), SerializeField, InspectorTooltip("Somente se a quantidade especifica de elementos estiver nos slots desse grupo é que ele estará pronto para ser validado. Caso contrário, com apenas um elemento já fica liberado para tentar validar")]
		protected bool _enableValidationOnlyIfSpecifcAmount;
		protected bool ShowEnableValidation
		{
			get
			{
				return (SpecificAmount.HasValue && !Overwritten);
			}
		}

		[SerializeField, InspectorTooltip("Lista de labels que serão recusados.")]
		protected List<string> _invalidLabels;

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
					if (readyCount == SpecificAmount)
						ret = true;
					else
						ret = false;
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



            if (_isGeneric)
            {
                var labels = temp[0].draggableReference.Labels;

                foreach (var l in labels)
                {
                    ret = temp.All(s => s.Validate(l));
                    if (ret)
                    {
                        break;
                    }
                }
            }
            else
            {
                switch (OperationLogic)
                {
                    case OperationLogic.AND:
                        ret = temp.All(s => s.Validate(acceptableAnswer));
                        break;
                    case OperationLogic.OR:
                        ret = temp.Any(s => s.Validate(acceptableAnswer));
                        break;
                }

                if (ret && SpecificAmount.HasValue)
                {
                    int qtde = temp.Where(s => s.Validate(acceptableAnswer)).ToList().Count;
                    ret = (qtde == SpecificAmount.Value);
                }
            }
            return ret;
        }		

		#endregion

		public override bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
		{

			if(draggable.Labels.Any(l => _invalidLabels.Contains(l)))
				return false;


			bool ret = base.SetInSlot(draggable, ref slot);

			if(ret && _freezeAfterDrop)
			{
				Freeze(draggable.gameObject);
				Freeze(slot.gameObject);
			}

			if (ret && _fadeAndFreezeOnDrop)
			{
				LeanTween.alpha(draggable.gameObject, 0, _animationTime);
				LeanTween.scale(draggable.gameObject, Vector3.zero, _animationTime);
				Freeze(draggable.gameObject);
				Freeze(slot.gameObject);
			}

			return ret;
		}

		protected void Freeze(GameObject go)
		{
			go.GetComponent<Collider2D>().enabled = false;
		}


	}
}