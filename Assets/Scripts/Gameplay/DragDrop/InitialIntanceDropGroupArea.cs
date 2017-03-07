using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using UnityEngine;

namespace MDS.Gameplay.DragDrop
{
    public class Item
    {
        public DropGroupSlot slot;
        public Draggable[] drags;
        public int instanceAmount;
        public int index;
    }

    public class InitialIntanceDropGroupArea : InitialDropGroupArea
    {

		[SerializeField] private int? m_maxDraggables;
		private int[] m_currentPerDraggable;
		[InspectorComment("Usado em casos onde necessita-se trocar o parent. O clone fica parenteado pelo pai do drag modelo e este vai para um novo gameobject")]
		public bool changeParent;
		public bool changeItem;
        [InspectorShowIf("changeItem"), InspectorTooltip("Total de itens disponíveis para serem instanciados")]
        public Item[] itens;
        [SerializeField] private Transform dropedItens;

        public override bool SetInSlot(Draggable draggable, ref DropGroupSlot slot, float duration = 0.5f)
        {
            return false;
        }

        public override void Start()
        {
            if (changeItem)
            {
                for (int i = 0; i < itens.Count(); i++)
                {
                    Draggable newDraggable = null;
                    newDraggable = Instantiate(itens[i].drags[itens[i].index]);
                    newDraggable.gameObject.SetActive(true);
                    newDraggable.transform.SetParent(dropedItens);
                }
            }

            base.Start();

            foreach (var slot in slots)
            {
                if (slot.draggableReference!=null)
                {
                    if (slot.draggableReference.OnAfterDrop==null)
                    {
                        LogError("OnAfterDrop está nulo em " + slot.draggableReference.name +". Bug do inspector?");
                    }
                    slot.draggableReference.OnAfterDrop.AddListener(DraggableAfterDropHandler);
                    slot.draggableReference.instantiableDraggable = true;
                    if (slot.draggableReference.PreferredInitialIndex.HasValue==false)
                        Debug.LogError("Para um InitialIntanceDropGroupArea, todos os Draggables DEVEM ter um PreferredInitialIndex");
                }
                else
                    Debug.LogError("Para um InitialIntanceDropGroupArea, todos os slots precisam estar preenchidos");
            }

            //cria uma referencia pra contar a quantidade de cada draggable
            if (m_maxDraggables.HasValue) {
				m_currentPerDraggable = new int[transform.childCount];
				for (int i = 0; i < transform.childCount; i++) {
					m_currentPerDraggable [i] = m_maxDraggables.Value;
				}
			}
        }

        private void DraggableAfterDropHandler(Draggable draggable, DropGroupSlot originalSlot)
        {
            // Nao deveria cair nesse if uma vez que o metodo SetInSlot foi sobrescrito para nao deixar NADA ser derrubado sobre si mesmo..
            if (draggable.currentSlot.IsInitialSlot)
                return;

            // se saiu de um initial slot, entao ok.. caso contrario, so sair..
            if (originalSlot.IsInitialSlot == false)
                return;

			//Se tiver limite de draggable, pega a referencia criada no start e decrementa de acordo com o preferred initial index. Não deixa instanciar mais do draggable referenciado;
			if (m_maxDraggables.HasValue) {
				int index = draggable.PreferredInitialIndex.Value;
				--m_currentPerDraggable [index];
				if (m_currentPerDraggable [index] == 0)
					return;
			}
            
			Draggable newDraggable = null;

            if (changeItem)
            {
                Item originalItem = itens.Single(i => i.slot == originalSlot);
                originalItem.index++;
                if (originalItem.instanceAmount > 0 && originalItem.index == originalItem.instanceAmount)
                    return;
                if (originalItem.index >= originalItem.drags.Count())
                    originalItem.index = 0;
                newDraggable = Instantiate(originalItem.drags[originalItem.index]);
                newDraggable.gameObject.SetActive(true);
            }
            else
            {
                newDraggable = Instantiate(draggable);
            }

            newDraggable.name = draggable.name;
            draggable.OnAfterDrop.RemoveAllListeners();
            newDraggable.OnAfterDrop.AddListener(DraggableAfterDropHandler);
			newDraggable.GetComponent<Collider2D> ().enabled = true;
            newDraggable.enabled = true;

            if (dropedItens!=null)
                newDraggable.transform.SetParent(dropedItens);

			if (changeParent)
			{
				newDraggable.transform.SetParent(draggable.transform.parent);
				draggable.transform.SetParent(dropedItens);
			}


            if (draggable.scaleState!=null)
            {
                newDraggable.scaleState = new DraggableState<float>()
                {
                    draggingValue = draggable.scaleState.draggingValue,
                    releasedFinalPositionValue = draggable.scaleState.releasedFinalPositionValue,
                    releasedValue = draggable.scaleState.releasedValue
                };
            }

            if (draggable.spriteState!=null)
            {
                newDraggable.spriteState = new DraggableState<Sprite>()
                {
                    draggingValue = draggable.spriteState.draggingValue,
                    releasedFinalPositionValue = draggable.spriteState.releasedFinalPositionValue,
                    releasedValue = draggable.spriteState.releasedValue
                };
            }
                       
           // newDraggable.transform.localScale = Vector3.one * draggable.scaleState.releasedValue;

            // o metodo DraggableUtilities.SetDraggableInSlot altera as referencias entao nao pode ser usado..
            newDraggable.currentSlot = originalSlot;
			originalSlot.draggableReference = newDraggable;
			Vector3 pos = originalSlot.transform.position;
			newDraggable.TweenGoto (pos, 0);

            
        }

        public void ResetInitialInstanceGroup()
        {
            foreach (var item in itens)
            {
                if (item.slot.draggableReference != null)
                {
                    Destroy(item.slot.draggableReference.gameObject);
                    item.slot.draggableReference = null;
                }
                item.index = 0;
                Draggable newDraggable = null;
                newDraggable = Instantiate(item.drags[0]);
                newDraggable.gameObject.SetActive(true);
                newDraggable.transform.SetParent(dropedItens);


                newDraggable.OnAfterDrop.AddListener(DraggableAfterDropHandler);
                newDraggable.GetComponent<Collider2D>().enabled = true;
                newDraggable.currentSlot = item.slot;
                item.slot.draggableReference = newDraggable;
                Vector3 pos = item.slot.transform.position;
                newDraggable.TweenGoto(pos, 0);
            }
        }
    }
}
