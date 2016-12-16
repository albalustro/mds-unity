using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopySpriteFromMemorized_G3W4E4C4 : MDSBehaviour {

	void Start () {
		GameObject go = Instantiate(MemorizeMe.MemorizedGameObject.GetComponent<SpriteRenderer>()).gameObject;
		go.transform.position = transform.position;
		LeanTween.scale(go, Vector3.one, .5f).setEase(LeanTweenType.punch);
	}
	
}
