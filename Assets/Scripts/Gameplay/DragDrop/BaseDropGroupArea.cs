using UnityEngine;
using System.Linq;
using FullInspector;
using System;
using System.Collections.Generic;

namespace MDS.Gameplay.DragDrop
{

    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public abstract class BaseDropGroupArea : MDSBehaviour
    {

        protected List<DropGroupSlot> slots;

        public virtual bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
        {
            // se chegou aqui com slot nulo, vamos procurar um slot para ele. possivelmente soltou o draggable sobre
            //  o grupo e nao sobre um slot especifico.
            if(slot == null)
            {
                // Se o grupo em questão é inicial (nao da para saber atravez da classe base, por isso do hack)
                //  e o draggable tem um indice inicial e, ainda por cima, esse indice inicial é
                //  obrigatorio no OnDrop..
                if (slots[0].IsInitialSlot && draggable.PreferredInitialIndex.HasValue && draggable.forcePreferredIndexOnDrop)
                {
                    // Caso o slot necessario ja esteja ocupado, anulamos a variavel e deixamos o resto do codigo cuidar do caso..
                    slot = slots[draggable.PreferredInitialIndex.Value];
                    if(slot.IsTaken)
                        slot = null;
                }
                else // caso contrario, pega o primeiro que nao esteja ocupado e be happy..
                {
                    //slot = slots.FirstOrDefault(s => s.IsTaken == false);
                    slot = slots.FirstOrDefault(s => s.IsNotTakenAndHasAcceptableLabel(draggable.Labels));
                }
            }
            else
            {
                // Caso recebeu um slot como parametro, deve certificar de que esse slot
                // esta disponivel e os labels do draggable sao aceitaveis
                if(!slot.IsNotTakenAndHasAcceptableLabel(draggable.Labels))
                    slot = null;
            }

            // se, ainda assim, o slot continua nulo, significa que nao existe slot disponivel nesse grupo.. 
            if(slot == null)
                return false;

            // Ok, slot disponivel, go ahead..
            
            // Se o slot é do grupo inicial e o draggable tem um indice inicial preferencial
            // e esse indice é obrigatório...
            if(slot.IsInitialSlot && draggable.PreferredInitialIndex.HasValue && draggable.forcePreferredIndexOnDrop)
            {
                // se nao tiver soltado o draggable no slot certo, retorna falso.
                int slotIndex = slots.IndexOf(slot);
                if(slotIndex != draggable.PreferredInitialIndex.Value)
                {
                    return false;
                }
            }


            // todo:
            // da forma como está, ainda é possível 'forçar' um draggable no slot inicial errado 
            // simplesmente fazendo o swap (soltando um draggable em um slot final ocupado por outro draggable)
            DraggableUtilities.SetDraggableInSlot(draggable, slot);

            return true;

        }

        protected override void Awake()
        {
            base.Awake();
            gameObject.layer = LayerMask.NameToLayer("Group");
        }

        public virtual void Start()
        {
            FillSlots();
        }

        public void FillSlots()
        {
            slots = transform.GetComponentsInChildren<DropGroupSlot>(true).ToList();
        }


        #region Unity Editor Only

#if UNITY_EDITOR

        Color editorBoundColor = Color.cyan;
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

}