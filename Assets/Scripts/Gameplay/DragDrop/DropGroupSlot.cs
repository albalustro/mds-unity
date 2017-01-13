using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MDS.Validators;
using System;
using MDS.Validators.Interfaces;
using FullInspector;
using System.Linq;
using MDS.Core.Interfaces;

namespace MDS.Gameplay.DragDrop
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class DropGroupSlot : MDSBehaviour, IValidatable
	{
        [SerializeField]
        private IAction[] OnAfterDropValidDraggableActions;

		[SerializeField]
		private bool _changeSprite;

		[SerializeField, InspectorShowIf("_changeSprite")]
		private SlotState<Sprite> _sprite;


		[SerializeField]
		private List<string> _acceptableLabels;


		private SpriteRenderer _spriteRenderer;

		private Draggable _draggableReference;

		[HideInInspector]
		public Draggable draggableReference
		{
			get
			{
				return _draggableReference;
			}
			set
			{
				_draggableReference = value;
				if (_changeSprite)
				{
					if(_draggableReference == null)
						_spriteRenderer.sprite = _sprite.EmptyValue;
					else
                    	_spriteRenderer.sprite = _sprite.FilledValue;
				}
                if(_draggableReference != null)
                    ExecuteActions(OnAfterDropValidDraggableActions);
			}
		}

		[HideInInspector]
		public bool IsInitialSlot { get; internal set; }

		[HideInInspector]
		public bool IsInstatiableInitialSlot { get; internal set; }

		public bool notNullIfNumeric;
		public int numericMultiplier;

		protected override void Awake()
		{
			base.Awake();

			base.gameObject.layer = LayerMask.NameToLayer("GroupSlot");

			GetComponent<BoxCollider2D>().isTrigger = true;

			if(GetComponentInParent<InitialDropGroupArea>() != null)
				IsInitialSlot = true;
			else
				IsInitialSlot = false;

			if(GetComponentInParent<InitialIntanceDropGroupArea>() != null)
				IsInstatiableInitialSlot = true;
			else
				IsInstatiableInitialSlot = false;

			if (_changeSprite)
			{
				_spriteRenderer = GetComponent<SpriteRenderer>();
				if(_spriteRenderer == null)
				{
                    _spriteRenderer = base.gameObject.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingLayerName = "DropGroup";
					_spriteRenderer.sortingOrder = 1;
				}
				_spriteRenderer.sprite = _sprite.EmptyValue;
			}
		}

		public bool IsTaken
		{
			get
			{
				return draggableReference != null;
			}
		}

		public bool IsNotTakenAndHasAcceptableLabel(List<string> labels)
		{
			if(IsTaken)
				return false;

            return AreAcceptableLabels(labels);

		}

		public bool HasLabelsToBeValidated()
		{
			return (_acceptableLabels != null && _acceptableLabels.Count > 0);
		}

		public bool IsAcceptableLabel(string label)
		{
			if(!HasLabelsToBeValidated())
				return true;

			return _acceptableLabels.Contains(label);
		}

        public bool AreAcceptableLabels(List<string> labels)
        {
            if(!HasLabelsToBeValidated())
                return true;

            if(labels.Any(l => IsAcceptableLabel(l)))
                return true;

            return false;
        }


		#region IValidatable

		public bool ReadyToValidate()
		{
			return IsTaken;
		}

		public bool Validate(string acceptableAnswer)
		{
			if(!ReadyToValidate())
				return false;

			if(string.IsNullOrEmpty(acceptableAnswer) || acceptableAnswer.Equals("*"))
				return true;

			return draggableReference.Labels.Contains(acceptableAnswer);
		}

		public int? GetNumericValue()
		{
			int result;
			if(IsTaken && int.TryParse(draggableReference.Labels[0], out result))
			{
				if (numericMultiplier != 0) {
					result *= numericMultiplier;
				}
				return result;
			}
			return null;
		}

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        #endregion

        #region Unity Editor Only

#if UNITY_EDITOR
        Color editorBoundColor = Color.yellow;
		void OnDrawGizmos()
		{
			BoxCollider2D box = GetComponent<BoxCollider2D>();
			if(box != null)
			{
				Gizmos.color = editorBoundColor;
				Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
			}
		}

		
#endif

		#endregion
	}

	public class SlotState<T>
	{
		public T FilledValue { get; set; }
		public T EmptyValue { get; set; }
	}

}