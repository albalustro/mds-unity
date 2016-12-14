using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using FullInspector;
using System.Collections.Generic;
using UnityEngine.Events;
using MDS.Core.Interfaces;

namespace MDS.Gameplay.DragDrop
{
	[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
	public class Draggable : MDSBehaviour
	{
		[InspectorTooltip("Se o draggable tiver um indice preferencial na hora de ser ajustado no group inicial, use esse campo")]
		public int? PreferredInitialIndex;

		private bool ShowForcePreferredIndex
		{
			get
			{
				return (PreferredInitialIndex.HasValue);
			}
		}

		[InspectorShowIf("ShowForcePreferredIndex"), InspectorTooltip("Esse atributo fará com que apenas o slot de indice = PreferredInitialIndex receba esse draggable")]
		public bool forcePreferredIndexOnDrop;

		public List<string> Labels;

		public bool changeSprite;
		public bool changeScale;

		[InspectorShowIf("changeSprite"), InspectorTooltip("Sprites usadas em cada estágio do processo de drag & drop. Caso o releasedFinalPositionValue seja nulo, o sprite releasedValue será usado.")]
		public DraggableState<Sprite> spriteState;

		[InspectorShowIf("changeScale"), InspectorTooltip("Valores de escala do sprite em cada estágio do processo de drag & drop. Caso o releasedFinalPositionValue seja 0, o valor releasedValue será usado.")]
		public DraggableState<float> scaleState;

		private Vector3 _touchOffset;
		private SpriteRenderer _renderer;

		[HideInInspector]
		public DropGroupSlot currentSlot;

		[HideInInspector]
		public bool instantiableDraggable;

        public UnityEvent<Draggable, DropGroupSlot> OnAfterDrop;

        public IAction[] OnAfterDropActions;

		protected override void Awake()
		{
			base.Awake();

			gameObject.layer = LayerMask.NameToLayer("Draggable");

			_renderer = this.GetComponent<SpriteRenderer>();


			if (changeScale && (scaleState.draggingValue <= 0f
						|| scaleState.releasedValue <= 0)) {
				scaleState.draggingValue = scaleState.releasedValue = 1f;
			}

			if(changeScale && scaleState.releasedFinalPositionValue == 0)
				scaleState.releasedFinalPositionValue = scaleState.releasedValue;

			if(changeSprite && spriteState.releasedFinalPositionValue == null)
				spriteState.releasedFinalPositionValue = spriteState.releasedValue;


			
		}

		public void OnMouseDown()
		{
			_renderer.sortingOrder = 5;
			_touchOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

			if(changeScale)
				LeanTween.scale(gameObject, Vector3.one * scaleState.draggingValue, 0.5f).setEase(LeanTweenType.easeOutElastic);

			if(changeSprite)
				_renderer.sprite = spriteState.draggingValue;

		}

		public void OnMouseDrag()
		{
			Vector3 curVer = new Vector3();
			Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _touchOffset;
			transform.position = Vector3.SmoothDamp(transform.position, newPos, ref curVer, 0.05f);
		}

		public void OnMouseUp()
		{

			BaseDropGroupArea group = null;
			DropGroupSlot slot = null;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 10;
			Vector2 screenPos = Camera.main.ScreenToWorldPoint(mousePos);
			RaycastHit2D hitGroup = Physics2D.Raycast(screenPos, Vector2.zero, 20f,
														LayerMask.GetMask(new[] { "Group" }));

			// Verifica se soltou sobre um group qualquer
			if(hitGroup)
			{
				group = hitGroup.transform.GetComponent<BaseDropGroupArea>();

				RaycastHit2D hitSlot = Physics2D.Raycast(screenPos, Vector2.zero, 20f,
												LayerMask.GetMask(new[] { "GroupSlot" }));

				// verifica se soltou sobre um slot especifico
				if(hitSlot)
				{
					slot = hitSlot.transform.GetComponent<DropGroupSlot>();

					// se o slot é um slot do grupo inicial e o draggable é forçado a voltar para
					// o slot de indice preferencial, anula o slot encontrado
					if(slot.IsInitialSlot && forcePreferredIndexOnDrop)
						slot = null;

				}


				// se for um draggable que saiu de um [initial intantiable group] e foi derrubado
				//  sobre um slot específico e esse slot já esta ocupado, *não* deve fazer swap entre 
				//  os elementos, que é comportamento padrao. 
				// Porem tb nao deve procurar um slot vazio.
				if( currentSlot.IsInstatiableInitialSlot && slot && slot.IsTaken)
				{
					TweenGoto(currentSlot.transform.position);
					return;
				}

				// caso o grupo aceite o draggable, invocar o evento
				// entao deve voltar para a posicao de onde saiu.
				DropGroupSlot originalSlot = currentSlot;
				if(group.SetInSlot(this, ref slot))
                {
                    ProcessSlotChanging(originalSlot);
                }
                else
				// caso contrário, (por motivos quaisquer) o group nao aceitar o draggable, entao deve voltar para a posicao que estava
				{
					TweenGoto(currentSlot.transform.position);
				}


			}
			else
			{ // soltou fora de grupos
				// se for um draggable advindo de um instantiable initial group E nao estava no slot inicial
				// entao deve ser destruido..

				if( instantiableDraggable && !currentSlot.IsInstatiableInitialSlot)
				{
					currentSlot.draggableReference = null;
					FadeAndDestroy();
				}
				else
				{
					TweenGoto(currentSlot.transform.position);
				}
			}

		}

        public void ProcessSlotChanging(DropGroupSlot originalSlot)
        {
            if(OnAfterDrop != null)
                OnAfterDrop.Invoke(this, originalSlot);

            ExecuteActions(OnAfterDropActions);
        }

        public void TweenGoto(Vector3 pos, float speed = 0.5f)
		{
			_renderer.sortingOrder = 5;
			LeanTween.move(gameObject, pos, speed)
				.setEase(LeanTweenType.easeOutCubic)
				.setOnComplete(() =>
				{
					_renderer.sortingOrder = 0;
					if(changeSprite)
					{
						if(!currentSlot.IsInitialSlot)
							_renderer.sprite = spriteState.releasedFinalPositionValue;
						else
							_renderer.sprite = spriteState.releasedValue;
					}


					if(changeScale)
					{
						float value = scaleState.releasedValue;
						if(!currentSlot.IsInitialSlot)
							value = scaleState.releasedFinalPositionValue;
						LeanTween.scale(gameObject, Vector3.one * value, 0.2f).setEase(LeanTweenType.linear);
					}
					pos.z = -1;
					transform.position = pos;
				});
		}

		public void FadeAndDestroy()
		{
			_renderer.sortingOrder = 5;

			LeanTween.alpha(gameObject, 0, 0.25f);
			LeanTween.scale(gameObject, Vector3.zero, 0.25f)
				.setEase(LeanTweenType.easeOutCubic)
				.setOnComplete(() =>
				{
					_renderer.sortingOrder = 0;
					Destroy(gameObject);
				});
		}

#if UNITY_EDITOR
		Color editorBoundColor = Color.blue;
		void OnDrawGizmos()
		{
			BoxCollider2D box = GetComponent<BoxCollider2D>();
			if(box != null)
			{
				Gizmos.color = editorBoundColor;
				Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
			}

            var bounds = GetComponent<Renderer>().bounds;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

		}
#endif

	}

}