using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintScreen : MonoBehaviour
{
	public int counter;
	public float gameViewZoomFactor = 1f;
	public string prefix = "MDS";

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			Capture();
	}

	private void Capture()
	{
		counter++;
		float superSize = 1f / gameViewZoomFactor;
		string scale = Screen.width.ToString() + "x" + Screen.height;
		string screenSize = "12.9";
		if (Screen.width == 2208)
			screenSize = "5.5";
		if (Screen.width == 2688)
			screenSize = "6.5";
		string fileName = string.Format("ScreensShots/Store/{0}/{3}/{0}_{1}_{2}.png", prefix, scale, counter, screenSize);
		Debug.Log(fileName);
		ScreenCapture.CaptureScreenshot(fileName);
	}
}
