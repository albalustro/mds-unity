using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FullInspector;
using MDS.Core.Interfaces;
using MDS.Gameplay.DragDrop;
using UnityEngine;

public class Recipe : MDSBehaviour {

    public bool _debugActions;

    [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    public IAction[] _onRecipeComplete;

	DropGroupSlot[] _slots;
    bool _recipeCompleted;

    

	protected override void Awake()
	{
		base.Awake();
		_slots = GetComponentsInChildren<DropGroupSlot>();
        _recipeCompleted = false;

    }

	void Update ()
    {

        if(_recipeCompleted) return;

        if (_debugActions)
        {
            _recipeCompleted = true;
            ExecuteActions(_onRecipeComplete);
        }

        if (_slots.All(s=>s.draggableReference!=null))
		{
            _recipeCompleted = true;
			ExecuteActions(_onRecipeComplete);
		}
	}
}
