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


            Draggable newDraggable = Instantiate(draggable);
            newDraggable.OnAfterDrop += DraggableAfterDropHandler;


            // o metodo DraggableUtilities.SetDraggableInSlot altera as referencias entao nao pode ser usado..
            newDraggable.currentSlot = originalSlot;
            originalSlot.draggableReference = newDraggable;
            Vector3 pos = originalSlot.transform.position;
            newDraggable.TweenGoto(pos, 0);
        }
        
    }
}
