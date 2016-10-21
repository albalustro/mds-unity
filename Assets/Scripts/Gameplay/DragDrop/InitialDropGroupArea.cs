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

            if(slots == null || slots.Length == 0)
                return;

            int amountSlots = slots.Length;
            int amountDraggables = draggablesInScene.Length;

            int maxIndex = Math.Min(amountSlots, amountDraggables);

            for(int i = 0; i < maxIndex; i++)
            {
                draggablesInScene[i].currentSlot = slots[i];
                slots[i].draggableReference = draggablesInScene[i];
                Vector3 pos = slots[i].transform.position;
                pos.z = -1;
                draggablesInScene[i].transform.position = pos;
            }

            if (amountSlots<amountDraggables)
                Debug.LogError("Há mais draggables doque slots no grupo inicial"); ;
        }
    }
}
