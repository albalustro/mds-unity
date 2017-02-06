using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipMediadorByPlayerPosition : MonoBehaviour {

	private Transform playerTransform;
	private Vector3 targetPosition;
	private SpriteRenderer renderer;
	[FullInspector.InspectorTooltip("Set this to only flip GO if its Mediator")]
	private bool _isMediator;

	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		renderer = GetComponent<SpriteRenderer> ();
		StartCoroutine (CheckFlip ());
		if (gameObject.tag == "Mediator")
			_isMediator = true;
			
	}

	IEnumerator CheckFlip () {
		while (true)
		{
			targetPosition = playerTransform.position;
			if (_isMediator) {
				FlipMediator ();
			}

			if ((targetPosition.y - 0.5f) < (transform.position.y - 1f))
				renderer.sortingOrder = 0;
			else
				renderer.sortingOrder = 2;
			
			yield return new WaitForSeconds (0.35f);
		}
	}

	void FlipMediator()
	{
		if (targetPosition.x < transform.position.x)
			renderer.flipX = false;
		else
			renderer.flipX = true;
	}
}
