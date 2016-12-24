using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBG : MonoBehaviour {

	[SerializeField]
	private Transform[] _bgs;
	[SerializeField]
	private Transform _beginRef;
	[SerializeField]
	private Transform _endRef;
	[SerializeField]
	private float _movingSpeed;

	public float MovingSpeed
	{
		get { return _movingSpeed; }
		set { _movingSpeed = value; }
	}

	void Update () {
		float displace = _movingSpeed * Time.deltaTime;
		foreach(var bg in _bgs)
		{
			bg.position = bg.position - new Vector3(displace, 0f, 0f);
			if(bg.position.x < _endRef.position.x)
			{
				bg.position = new Vector3(_beginRef.position.x, bg.position.y, bg.position.z);
			}
		}
	}
}
