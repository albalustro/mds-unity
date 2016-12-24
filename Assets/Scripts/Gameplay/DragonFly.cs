using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFly : MDSBehaviour {

	[SerializeField]
	private ScrollBG _scrollBG;
	private ContinuousMove _continuousMove;
	private float normalSpeed;
	public float slowSpeed;

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
