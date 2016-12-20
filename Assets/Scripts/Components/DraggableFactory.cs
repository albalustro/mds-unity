using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using MDS.Gameplay.DragDrop;
using MDS.Utilities;
using UnityEngine;

public class DraggableFactory : MDSBehaviour
{
    [SerializeField]
    private int? _amount;
    private bool HasAmount { get { return _amount.HasValue; } }
    private int _currentAmount;
    [SerializeField, FullInspector.InspectorShowIf("HasAmount")]
    private IAction[] OnFinishActions;


    [SerializeField]
    private float _spawnInterval = 3f;


    [SerializeField]
    private Draggable[] _draggablePrefabs;

    [SerializeField]
    private DropGroupSlot _slotPrefab;

    [SerializeField]
    private Transform[] _instantiatePositionRef;


    public IEnumerator Start()
    {

        while(true)
        {
            yield return new WaitForSeconds(_spawnInterval);

            if(_amount.HasValue)
            {
                _currentAmount++;
                if(_currentAmount == _amount.Value)
                {
                    ExecuteActions(OnFinishActions);
                    yield break;
                }
            }


            var drag = Instantiate(_draggablePrefabs.GetRandom());
            drag.transform.position = _instantiatePositionRef.GetRandom().position;
            drag.instantiableDraggable = true;

            var slot = Instantiate(_slotPrefab, drag.transform, false);
            slot.GetComponent<Collider2D>().enabled = false;

            drag.currentSlot = slot;
            slot.draggableReference = drag;


        }

    }
}
