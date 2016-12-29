using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerEpisodeController : MDSBehaviour {

    PlayerAnimController _animController;
    PolyNavAgent _agent;

    public void Move(Vector2 position)
    { 
        _agent.SetDestination(position, StopWalkAnim);
    }

    private void Update()
    {
        if(_agent.currentSpeed < 0.1) return;

        _animController.SetWalking(true, _agent.movingDirection);
    }

    private void StopWalkAnim(bool obj)
    {
        _animController.SetWalking(false, Vector2.zero);
    }

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<PolyNavAgent>();
        _animController = GetComponent<PlayerAnimController>();
    }

   

}
