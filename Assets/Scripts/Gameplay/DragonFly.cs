using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Actions;
using MDS.Core.Interfaces;

public class DragonFly : MDSBehaviour {

	[SerializeField]
	private ScrollBG _scrollBG;
	private ContinuousMove _continuousMove;
	private float normalSpeed;
	public float slowSpeed;

	[SerializeField]
	private IAction[] m_onCollisionAction;

	private bool m_playerHasBeenWarned = false;

	void Start()
	{
		normalSpeed = _scrollBG.MovingSpeed;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag ("Arvore"))
		{
			_continuousMove = other.GetComponent<ContinuousMove> ();
			StartCoroutine (ReduceSpeed ());

			if (!m_playerHasBeenWarned) {
				ExecuteActions (m_onCollisionAction);
			}

			m_playerHasBeenWarned = true;

		}
	}

	IEnumerator ReduceSpeed()
	{
		_continuousMove.CurrentSpeed = slowSpeed;
		_scrollBG.MovingSpeed = slowSpeed;
		yield return new WaitForSeconds (2.5f);
		_continuousMove.CurrentSpeed = normalSpeed;
		_scrollBG.MovingSpeed = normalSpeed;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag ("Arvore"))
		{
			_continuousMove = other.GetComponent<ContinuousMove> ();
			_continuousMove.CurrentSpeed = normalSpeed;
			_scrollBG.MovingSpeed = normalSpeed;
		}
	}

}
