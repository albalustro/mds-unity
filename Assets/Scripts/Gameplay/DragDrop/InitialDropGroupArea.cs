using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using UnityEngine;

namespace MDS.Gameplay.DragDrop
{
    public class InitialDropGroupArea :  BaseDropGroupArea
    {
        public override void Start()
        {
            base.Start();

            SetupDraggablesInitialPosition();
        }

        [InspectorButton, InspectorTooltip("Use esse recurso para posicionar corretamente os draggables")]
        public void SetupDraggablesInitialPosition()
        {
            Draggable[] draggablesInScene = FindObjectsOfType<Draggable>();

            if(draggablesInScene == null || draggablesInScene.Length == 0)
            {
                Debug.LogError("Nenhum draggable encontrado na cena");
                return;
            }

            if(slots == null || slots.Count == 0)
                return;

            int amountSlots = slots.Count;
            int amountDraggables = draggablesInScene.Length;
            int maxIndex = Math.Min(amountSlots, amountDraggables);

            foreach(var d in draggablesInScene)
            {
                d.currentSlot = null;
            }

            foreach(var s in slots)
            {
                s.draggableReference = null;
            }

            if(draggablesInScene.Any(d => d.PreferredInitialIndex.HasValue))
            {


                var repeatedPreferredIndex = draggablesInScene
                                                .Where(w=>w.PreferredInitialIndex.HasValue)
                                                .GroupBy(g => g.PreferredInitialIndex.Value)
                                                .Where(g => g.Count() > 1).Count();

                if (repeatedPreferredIndex>0)
                {
                    Debug.LogError("Há draggable com PreferredInitialIndex repetido. Revise.");
                    return;
                }


                if (draggablesInScene.Any(d=> d.PreferredInitialIndex.HasValue && d.PreferredInitialIndex.Value >= amountSlots))
                {
                    Debug.LogError("Há dragabble com PreferredInitialIndex maior que a qtde de slots. Revise.");
                    return;
                }

                int p_index;
                Vector3 pos;
                foreach(var d in draggablesInScene.Where(w=>w.PreferredInitialIndex.HasValue))
                {
                    p_index = d.PreferredInitialIndex.Value;
                    d.currentSlot = slots[p_index];
                    slots[p_index].draggableReference = d;
                    pos = slots[p_index].transform.position;
                    pos.z = -1;
                    d.transform.position = pos;
                }

            }

            int freeSlotIndex = 0;
            for(int i = 0; i < maxIndex; i++)
            {
                if (draggablesInScene[i].currentSlot!=null)
                    continue;
                while(slots[freeSlotIndex].IsTaken)
                    freeSlotIndex++;
                draggablesInScene[i].currentSlot = slots[freeSlotIndex];
                slots[freeSlotIndex].draggableReference = draggablesInScene[i];
                Vector3 pos = slots[freeSlotIndex].transform.position;
                pos.z = -1;
                draggablesInScene[i].transform.position = pos;

            }

            if (amountSlots<amountDraggables)
                Debug.LogError("Há mais draggables que slots no grupo inicial"); ;
        }
    }
}
