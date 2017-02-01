using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Core;
using MDS.Core.Interfaces;
using FullInspector;
using UnityEngine.SceneManagement;

public class OnSceneWasLoadedAction : MDSBehaviour {

    [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    public IAction[] actionToRun;


	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		ExecuteActions(actionToRun);
	}
}
