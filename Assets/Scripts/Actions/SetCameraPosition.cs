using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Actions;
using MDS.Core.Interfaces;

public class SetCameraPosition : BaseAction {

	[SerializeField]
	private bool _changeCamPosition;

	[SerializeField, FullInspector.InspectorShowIf("_changeCamPosition")]
	private Vector3 _camNewPosition;

	[SerializeField]
	private float? _newRightX, _newLeftX, _newTopY, _newBottomY;

	public override IEnumerator Execute()
	{
		if(byPass) yield break;
		yield return base.Execute();

		EpisodeSceneManager sceneManager = GameObject.FindObjectOfType<EpisodeSceneManager> ();

		if (sceneManager != null) {

			if(_changeCamPosition)
				sceneManager.SetCameraPosition (_camNewPosition);

			if (_newRightX.HasValue)
				sceneManager.rightX = _newRightX.Value;

			if (_newLeftX.HasValue) {
				sceneManager.leftX = _newLeftX.Value;
			}

			if (_newTopY.HasValue)
				sceneManager.topY = _newTopY.Value;

			if (_newBottomY.HasValue)
				sceneManager.bottomY = _newBottomY.Value;

		}

	}

}
