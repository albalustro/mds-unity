using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MDSBehaviour {

	[SerializeField]
	private Transform[] _rightMovingBelt;
	[SerializeField]
	private Transform[] _leftMovingBelt;

	[SerializeField]
	private Transform _movingBeltRightNearRef;
	[SerializeField]
	private Transform _movingBeltRightFarRef;
	[SerializeField]
	private Transform _movingBeltLeftNearRef;
	[SerializeField]
	private Transform _movingBeltLeftFarRef;

	[SerializeField]
	private Transform[] _clockWiseRotatingBearing;
	[SerializeField]
	private Transform[] _antiClockWiseRotatingBearing;

	[SerializeField]
	private float _movingSpeed;
	[SerializeField]
	private float _rotatingSpeed;

	void Update () {

		float displace = _movingSpeed * Time.deltaTime;
		float rot = _rotatingSpeed * Time.deltaTime;

		foreach(var belt in _rightMovingBelt)
		{
			belt.position = belt.position + new Vector3(displace, 0f, 0f);
			if(belt.position.x > _movingBeltRightNearRef.position.x)
			{
				belt.position = new Vector3(_movingBeltLeftFarRef.position.x, belt.position.y, belt.position.z);
			}
		}

		foreach(var belt in _leftMovingBelt)
		{
			belt.position = belt.position - new Vector3(displace, 0f, 0f);
			if(belt.position.x < _movingBeltLeftNearRef.position.x)
			{
				belt.position = new Vector3(_movingBeltRightFarRef.position.x, belt.position.y, belt.position.z);
				
			}
		}

		foreach(var bearing in _clockWiseRotatingBearing)
		{
			bearing.Rotate(0, 0, -rot, Space.Self);
		}

		foreach(var bearing in _antiClockWiseRotatingBearing)
		{
			bearing.Rotate(0, 0, rot, Space.Self);
		}
	}
}
