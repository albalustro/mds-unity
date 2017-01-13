using System;
using System.Collections;
using System.Collections.Generic;
using MDS.Interactable;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerEpisodeController : MDSBehaviour {

    PlayerAnimController _animController;
    PolyNavAgent _agent;
    InteractableBase _interactable;

    public void MoveAndInteract(Vector2 position, InteractableBase interactable)
    {
        _agent.SetDestination(position, StopWalkAnim);
        _interactable = interactable;
    }

    public void Move(Vector2 position)
    { 
        _agent.SetDestination(position, StopWalkAnim);
        _interactable = null;
    }

    private void Update()
    {
        if(_agent.currentSpeed < 0.1) return;

        _animController.SetWalking(true, _agent.movingDirection);
    }

    private void StopWalkAnim(bool obj)
    {
        _animController.SetWalking(false, Vector2.zero);

        if(_interactable != null)
        {
            _interactable.Interact();
            _interactable = null;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<PolyNavAgent>();
        _animController = GetComponent<PlayerAnimController>();
    }

   

}
