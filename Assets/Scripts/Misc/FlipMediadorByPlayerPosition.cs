using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipMediadorByPlayerPosition : MonoBehaviour {

	private Transform playerTransform;
	private Vector3 targetPosition;
	private SpriteRenderer renderer;

	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		renderer = GetComponent<SpriteRenderer> ();
		StartCoroutine (CheckFlip ());
	}

	IEnumerator CheckFlip () {
		while (true)
		{
			targetPosition = playerTransform.position;
			if (targetPosition.x < transform.position.x)
				renderer.flipX = false;
			else
				renderer.flipX = true;

			if ((targetPosition.y - 0.5f) < (transform.position.y - 1f))
				renderer.sortingOrder = 0;
			else
				renderer.sortingOrder = 2;
			
			yield return new WaitForSeconds (0.35f);
		}
	}
}
