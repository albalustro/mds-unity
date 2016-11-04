using UnityEngine;
using System.Collections;
using MDS.Validators.Interfaces;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class CarrousselWindow : MDSBehaviour
{
    private CarrousselItem _carrousseItem;

    public void Start()
    {
        _carrousseItem = GetComponentInChildren<CarrousselItem>();
        if(_carrousseItem == null)
            Debug.LogError("CarrousselWindow sem CarrousselItem filho");
    }

    public void OnMouseUp()
    {
        _carrousseItem.MoveNext();
    }

}
