using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipMediadorByPlayerPosition : MonoBehaviour {

	private Transform playerTransform;
	private Vector3 targetPosition;
	private SpriteRenderer _renderer;
	[FullInspector.InspectorTooltip("Set this to only flip GO if its Mediator")]
	private bool _isMediator;

	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform.FindChild("Art").GetComponent<Transform> ();
		_renderer = GetComponent<SpriteRenderer> ();
		StartCoroutine (CheckFlip ());
		if (gameObject.tag == "Mediator")
			_isMediator = true;
			
	}

    Vector3 myPos;
	IEnumerator CheckFlip () {
        while(true)
        {
            myPos = transform.position;
            targetPosition = playerTransform.position;
            if(_isMediator)
            {
                FlipMediator();
            }

            //if((targetPosition.y - 0.5f) < (transform.position.y - 1f))
            if(targetPosition.y < transform.position.y)
                myPos.z = targetPosition.z + 0.1f;
            //_renderer.sortingOrder = -1 ;
            else
                myPos.z = targetPosition.z - 0.1f;
            //_renderer.sortingOrder = 2;
            transform.position = myPos;
            yield return null;
        }
	}

	void FlipMediator()
	{
		if (targetPosition.x < transform.position.x)
			_renderer.flipX = false;
		else
			_renderer.flipX = true;
	}
}
