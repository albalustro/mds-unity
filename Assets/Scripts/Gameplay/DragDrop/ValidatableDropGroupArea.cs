using UnityEngine;
using System.Linq;
using MDS.Validators.Interfaces;
using MDS.Validators.Enum;
using FullInspector;
using System.Collections.Generic;
using System;
using MDS.Core.Interfaces;

namespace MDS.Gameplay.DragDrop
{
	[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
	public class ValidatableDropGroupArea : BaseDropGroupArea, IValidatableGroup, IValidatable
	{
	 
		[InspectorMargin(10), InspectorDivider, InspectorHeader("Validation Properties")]

		[InspectorCategory("Validation")]
		[SerializeField, InspectorOrder(0), InspectorTooltip("Com essa propriedade ativa, a validação usará um algoritmo diferente: o primeiro elemento define o LABEL que TODOS OS DEMAIS deverão ter para que a validação tenha sucesso. Caso o primeiro elemento tenha mais de um label, todos serão testados da mesma maneira")]
		private bool _isGeneric;

		[HideInInspector]
		//Usado propriedade para nao alterar o acesso (privado) da variavel
		public bool isGeneric { 
			get { return _isGeneric; }
			set { _isGeneric = value; }
		}
		
		[InspectorCategory("Validation")]
		public bool Overwritten { get; set; }

		[InspectorCategory("Validation")]
		[InspectorHideIf("Overwritten"), InspectorTooltip("Define a operação entre a validação dos elementos do grupo. Use AND para que todos os elementos sejam válidos e OR para que ao menos UM elemento seja válido para considerar o grupo válido")]
		public OperationLogic OperationLogic { get; set; }

		[InspectorCategory("Validation")]
		[InspectorHideIf("Overwritten"), InspectorTooltip("Ao usar essa propriedade, os slots que estiverem vazios (sem um draggable associado) serão filtrados ANTES da avaliação.")]
		public bool AcceptEmptyAsCorrectAnswer { get; set; }

		[InspectorCategory("Validation"), InspectorTooltip("Ao usar essa propriedade os o slots cujos gameObjects estiverem desabilitados serão filtrados ANTES da avaliação.")]
		public bool IgnoreDisabledSlots { get; set; }

		[InspectorCategory("Validation")]
		[InspectorHideIf("Overwritten")]
		public int? SpecificAmount { get; set; }

		[InspectorMargin(15), InspectorDivider, InspectorHeader("Mechanics Properties")]

		[InspectorCategory("Mechanics")]
		[SerializeField, InspectorTooltip("Selecione para fazer com que o objeto sofra um FADE OUT OnDrop")]
		protected bool _fadeAndFreezeOnDrop;

		[InspectorCategory("Mechanics")]
		[InspectorShowIf("_fadeAndFreezeOnDrop")]
		[InspectorRange(0.1f,1.0f), SerializeField, InspectorTooltip("Tempo, em segundos, para a animação de fade ocorrer")]
		protected float _animationTime;

		[InspectorCategory("Mechanics")]
		[InspectorShowIf("_fadeAndFreezeOnDrop")]
		[SerializeField, InspectorTooltip("Selecione para que o drop group se comporte como se fosse 'infinito'")]
		protected bool _infinityBag;

		[InspectorCategory("Mechanics")]
		[InspectorHideIf("_fadeAndFreezeOnDrop")]
		[SerializeField, InspectorTooltip("Selecione para fazer com que o objeto e o slot fiquem bloqueados após um draggable ser solto sobre esse grupo")]
		protected bool _freezeAfterDrop;

        [InspectorCategory("Validation"), SerializeField, InspectorTooltip("Com essa propriedade o grupo estará sempre validável, independente de seu conteúdo.")]
        protected bool _alwaysEnableValidation;

        [InspectorCategory("Validation")]
		[InspectorShowIf("ShowEnableValidation"), SerializeField, InspectorTooltip("Somente se a quantidade especifica de elementos estiver nos slots desse grupo é que ele estará pronto para ser validado. Caso contrário, com apenas um elemento já fica liberado para tentar validar")]
		protected bool _enableValidationOnlyIfSpecifcAmount;
		protected bool ShowEnableValidation
		{
			get
			{
				return (SpecificAmount.HasValue && !Overwritten && !_alwaysEnableValidation);
			}
		}

		[InspectorCategory("Mechanics")]
		[SerializeField, InspectorTooltip("Lista de labels que serão recusados. Se o draggable tiver um desses labels, não será aceito por esse grupo")]
		protected List<string> _invalidLabels;


		public override void Start()
		{
			base.Start();
			if(slots.Count > 1 && _infinityBag)
				Debug.LogError("Se o DropGroupArea é infinito, coloque apenas UM slot.");
		}

		public int Count(string label)
		{
			if(string.IsNullOrEmpty(label))
				return slots.Count(s => s.draggableReference != null);
			return slots.Count(s => s.draggableReference != null && s.draggableReference.Labels.Contains(label));
		}

		#region IValidatable

		public bool ReadyToValidate()
		{

            if(_alwaysEnableValidation)
                return true;

			bool ret = false;
			if (slots==null)
			{
				FillSlots();
				if(slots == null)
				{
                    Debug.LogErrorFormat(base.gameObject.name + " não possui slots.");
					return false;
				}
			}

			List<DropGroupSlot> temp = slots;

            if(AcceptEmptyAsCorrectAnswer)
                temp = temp.Where(s => s.draggableReference != null).ToList();

            if(IgnoreDisabledSlots)
                temp = temp.Where(s => s.gameObject.activeSelf == true).ToList();


            var readyCount = temp.Where(s =>  s.ReadyToValidate()).ToList().Count;


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
				ret = AcceptEmptyAsCorrectAnswer && !_enableValidationOnlyIfSpecifcAmount;
			}

			return ret;
		}

		public virtual bool Validate(string acceptableAnswer)
		{
			bool ret = false;

			List<DropGroupSlot> temp = slots;

			if(AcceptEmptyAsCorrectAnswer)
				temp = temp.Where(s => s.draggableReference != null).ToList();

			if (IgnoreDisabledSlots)
				temp = temp.Where(s => s.gameObject.activeSelf == true).ToList();

			switch(OperationLogic)
			{
				case OperationLogic.AND:
					ret = temp.All(s => s.Validate(acceptableAnswer));
					break;
				case OperationLogic.OR:
					ret = temp.Any(s => s.Validate(acceptableAnswer));
					break;
			}



			if(_isGeneric)
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
				if(SpecificAmount.HasValue)
				{
					int qtde = temp.Where(s => s.Validate(acceptableAnswer)).ToList().Count;
					ret = ret && (qtde == SpecificAmount.Value);
				}
				else
				{
					switch(OperationLogic)
					{
						case OperationLogic.AND:
							ret = temp.All(s => s.Validate(acceptableAnswer));
							break;
						case OperationLogic.OR:
							ret = temp.Any(s => s.Validate(acceptableAnswer));
							break;
					}
				}
				
			}
			return ret;
		}

		public int? GetNumericValue()
		{
			bool hasResult = false;
			int result = 0;
			int? temp;
			foreach(var s in slots)
			{
				temp = s.GetNumericValue();
				if (temp.HasValue) {
					result += temp.Value;
					hasResult = true;
				} else {
					if (s.notNullIfNumeric) {
						return null;
					}
				}
			}
			if (hasResult) {
				return result;
			}
			return null;
		}

		public GameObject GetGameObject()
		{
			return gameObject;
		}

		#endregion

		public override bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
		{

			if(draggable.Labels.Any(l => _invalidLabels.Contains(l)))
				return false;

			// TODO: melhorar isso.. ta uma porcaria..
			var organizer = GetComponent<GroupHOrganizer>();
			if(organizer != null)
				organizer.Organize(draggable);

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

				if(_infinityBag)
					DuplicateSlot(slot);

				Freeze(draggable.gameObject);
				Freeze(slot.gameObject);
			}

		   return ret;
		}

		protected void Freeze(GameObject go)
		{
			go.GetComponent<Collider2D>().enabled = false;
		}

		private void DuplicateSlot(DropGroupSlot slot)
		{
			DropGroupSlot newSlot = Instantiate(slot);
			slots.Add(newSlot);
			newSlot.transform.SetParent(transform,false);
		}

		public void ResetGroup()
		{
			//loop para destruir os dragreference de todos os slots e atribuir null
			foreach (var slot in slots)
			{
				if (slot.draggableReference != null)
				{
					Destroy(slot.draggableReference.gameObject);
					slot.draggableReference = null;
				}
			}
			//se for infinity...
			if (_infinityBag)
			{
				//loop para destruir todos os slots, mantendo 1
				for (int i = 1; i < slots.Count; i++)
				{
					Destroy(slots[i].gameObject);
					slots[i] = null;
				}
				slots.RemoveRange(1, slots.Count - 1);
				if (_fadeAndFreezeOnDrop)
				{
					slots[0].GetComponent<Collider2D>().enabled = true;
				}
			}
		}
		
	}
}