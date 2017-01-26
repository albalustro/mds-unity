using UnityEngine;
using System.Collections;
using MDS.Validators.Interfaces;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class CarrousselWindow : MDSBehaviour
{
    private CarrousselItem _carrousseItem;
	public bool isFreezed { get { return !GetComponent<Collider2D> ().enabled; } }

    public void Start()
    {
        _carrousseItem = GetComponentInChildren<CarrousselItem>();
        if(_carrousseItem == null)
            Debug.LogError("CarrousselWindow sem CarrousselItem filho");
        _carrousseItem.Initialize(this);
    }

    public void OnMouseUp()
    {
        _carrousseItem.MoveNext();
    }

    public void Freeze()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
