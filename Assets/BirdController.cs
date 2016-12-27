using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Core.Interfaces;
using FullInspector;



public class BirdController : MDSBehaviour {

	[SerializeField]
	private int m_counter = 2;

	public IAction[] m_onCounterZeroAction;

	private ContinuousMove m_move;
private bool trackMovement = true;

    protected override void Awake()
    {
        base.Awake();

        m_move = GameObject.FindObjectOfType<ContinuousMove>();
	}

	public void SetCounter()
	{
		--m_counter;
		m_move.m_freezeOnMaxDistance = true;
		if (m_counter <= 0) {
			m_move.m_freezeOnMaxDistance = true;
			ExecuteActions(m_onCounterZeroAction);
		}
	}

}
