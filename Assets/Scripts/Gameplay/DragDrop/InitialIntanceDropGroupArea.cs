using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using UnityEngine;

namespace MDS.Gameplay.DragDrop
{
    public class InitialIntanceDropGroupArea : InitialDropGroupArea
    {

		[SerializeField] private int? m_maxDraggables;
		private int[] m_currentPerDraggable;

//		[SerializeField] private Draggable[] m_newDraggable;
//		[SerializeField] private int m_newDragIndex;

        public override bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
        {
            return false;
        }

        public override void Start()
        {
            base.Start();

            foreach(var slot in slots)
            {
                if (slot.draggableReference!=null)
                {
                    slot.draggableReference.OnAfterDrop += DraggableAfterDropHandler;

                    slot.draggableReference.instantiableDraggable = true;

                    if (slot.draggableReference.PreferredInitialIndex.HasValue==false)
                    {
                        Debug.LogError("Para um InitialIntanceDropGroupArea, todos os Draggables DEVEM ter um PreferredInitialIndex");
                    }
                }
                else
                {
                    Debug.LogError("Para um InitialIntanceDropGroupArea, todos os slots precisam estar preenchidos");
                }
            }

			//cria uma referencia pra contar a quantidade de cada draggable
			if (m_maxDraggables.HasValue) {
				m_currentPerDraggable = new int[transform.childCount];
				for (int i = 0; i < transform.childCount; i++) {
					m_currentPerDraggable [i] = m_maxDraggables.Value;
				}
			}

//			if (m_newDraggable.Length != 0) {
//				m_newDragIndex = 0;
//			}

        }

        private void DraggableAfterDropHandler(Draggable draggable, DropGroupSlot originalSlot)
        {
            // Nao deveria cair nesse if uma vez que o metodo SetInSlot foi sobrescrito para nao deixar NADA 
            // ser derrubado sobre si mesmo..
            if(draggable.currentSlot.IsInitialSlot)
                return;

            // se saiu de um initial slot, entao ok.. caso contrario, so sair..
            if(originalSlot.IsInitialSlot == false)
                return;

			//Se tiver limite de draggable, pega a referencia criada no start e decrementa de acordo com o preferred initial index. Não deixa instanciar mais do draggable referenciado;
			if (m_maxDraggables.HasValue) {
				int index = draggable.PreferredInitialIndex.Value;
				--m_currentPerDraggable [index];
				if (m_currentPerDraggable [index] == 0)
					return;
			}

			Draggable newDraggable = null;

//			if (m_newDraggable.Length != 0) {
//				newDraggable = Instantiate (m_newDraggable [m_newDragIndex]);
//				++m_newDragIndex;
//				if (m_newDragIndex == m_newDraggable.Length) {
//					m_newDragIndex = 0;
//				}
//			} else {
				newDraggable = Instantiate (draggable);
//			}

			newDraggable.OnAfterDrop += DraggableAfterDropHandler;
			newDraggable.GetComponent<Collider2D> ().enabled = true;

			if (draggable.transform.parent != null)
				newDraggable.transform.SetParent (draggable.transform.parent);

			// o metodo DraggableUtilities.SetDraggableInSlot altera as referencias entao nao pode ser usado..
			newDraggable.currentSlot = originalSlot;
			originalSlot.draggableReference = newDraggable;
			Vector3 pos = originalSlot.transform.position;
			newDraggable.TweenGoto (pos, 0);

        }
        
    }
}
