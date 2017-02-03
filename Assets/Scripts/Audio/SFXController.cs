using System.Collections;
using UnityEngine;

public class SFXController : MDSBehaviour {

	public static SFXController instance;

	public AudioClip _won, _draggableDrag, _releaseInGroup, _releaseOutOfGroup, _inputClick;


	void Start()
	{
		instance = this;
	}


}
