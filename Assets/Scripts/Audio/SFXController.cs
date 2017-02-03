using System.Collections;
using UnityEngine;

public class SFXController : MDSBehaviour {

	public static SFXController instance;

	public AudioClip _won, _draggableDrag, _releaseInGroup, _releaseOutOfGroup, _inputClick, _carrousselChange;


	void Start()
	{
		instance = this;
	}


}
