using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Core.Interfaces;
using MDS.Utilities;

public class ItemSpawner : MDSBehaviour {

	[SerializeField]
	private int? _amount;
	private bool HasAmount { get { return _amount.HasValue; } }
	private int _currentAmount;
	[SerializeField, FullInspector.InspectorShowIf("HasAmount")]
	private IAction[] OnFinishActions;
	[SerializeField]
	private float _spawnInterval = 3f;
	[SerializeField]
	private GameObject[] _spawnPrefabs;
	[SerializeField]
	private Transform[] _instantiatePositionRef;
	private ContinuousMove currentMovingObj;

	public IEnumerator Start()
	{
		while(true)
		{
			yield return new WaitForSeconds(_spawnInterval);

			if(_amount.HasValue)
			{
				_currentAmount++;
				if(_currentAmount == _amount.Value)
				{
					ExecuteActions(OnFinishActions);
					yield break;
				}
			}
			var obj = Instantiate(_spawnPrefabs.GetRandom());
			obj.transform.position = _instantiatePositionRef.GetRandom().position;
			currentMovingObj = obj.GetComponent<ContinuousMove> ();
		}
	}

	void OnDisable()
	{
		if (currentMovingObj != null)
			currentMovingObj.enabled = false;
	}
}
