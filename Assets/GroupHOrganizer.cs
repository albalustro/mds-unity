using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MDS.Gameplay.DragDrop;
using UnityEngine;

public class GroupHOrganizer : MDSBehaviour {


    protected override void Awake()
    {
        base.Awake();

        var group = GetComponent<ValidatableDropGroupArea>();

        if(group == null)
            LogError("GroupHOrganizer deve ser usado em um DropGrouArea");

        var slots = GetComponentsInChildren<DropGroupSlot>();

        foreach(var slot in slots)
        {
            slot.GetComponent<Collider2D>().enabled = false;
        }

    }

    public void Organize(Draggable newDraggable)
    {

        var slots = GetComponentsInChildren<DropGroupSlot>();

        Vector3 position;
        //int firstEmpty=0;
        //int secondEmpty=0;

        //for(int i = 0; i < slots.Length-2; i++)
        //{
        //    if (slots[i].draggableReference == null)
        //    {
        //        firstEmpty = i;
        //        break;
        //    }
        //}

        //for(int i = firstEmpty+1; i < slots.Length-1; i++)
        //{
        //    if (slots[i].draggableReference==null)
        //    {
        //        secondEmpty = i;
        //        break;
        //    }
        //}

        //if (secondEmpty>0)
        //{
        //    for(int i = firstEmpty; i < slots.Length-1; i++)
        //    {
        //        DropGroupSlot firstSlot = slots[i];
        //        DropGroupSlot secondSlot = slots[i + 1];

        //        firstSlot.draggableReference = secondSlot.draggableReference;
        //        if (firstSlot.draggableReference != null)
        //        {
        //            firstSlot.draggableReference.currentSlot = firstSlot;
        //        }

        //    }
        //}

        bool hasEmpty = false;

        for(int i = 1; i < slots.Length; i++)
        {
            position = slots[i - 1].transform.position;
            position.z = -1;

            // slot anterior
            if(slots[i - 1].draggableReference != null)
                position += new Vector3(slots[i - 1].draggableReference.GetComponent<Renderer>().bounds.extents.x, 0, 0);
            else
            {
                if(!hasEmpty)
                {
                    position += new Vector3(newDraggable.GetComponent<Renderer>().bounds.extents.x, 0, 0);
                    hasEmpty = true;
                }
                else
                    position += new Vector3(.3f, 0, 0);
            }

            // slot atual
            if(slots[i].draggableReference != null)
                position += new Vector3(slots[i].draggableReference.GetComponent<Renderer>().bounds.extents.x, 0, 0);
            else
            {
                if (!hasEmpty)
                    position += new Vector3(newDraggable.GetComponent<Renderer>().bounds.extents.x, 0, 0);
                else
                {
                    position += new Vector3(.3f, 0, 0);
                }
            }


            slots[i].transform.position = position;
            if(slots[i].draggableReference != null)
                slots[i].draggableReference.transform.position = position;

        }

    }
	
    
}
