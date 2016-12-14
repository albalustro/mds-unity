using System.Collections;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;


[ExecuteInEditMode]
public class OrderInLayerByY : MDSBehaviour {

    private SpriteRenderer _sr;

    protected override void Awake()
    {
        base.Awake();
        _sr = GetComponent<SpriteRenderer>();
	}
	
	
	void Update () {
        int ord = (int)((transform.position.y+5)*10);
        _sr.sortingOrder = ord;
	}
}
