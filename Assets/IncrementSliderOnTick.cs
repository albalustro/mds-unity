using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementSliderOnTick : MDSBehaviour {

	private Coroutine _tick;
	private WaitForSeconds _seconds = new WaitForSeconds(1.5f);

	[SerializeField]
	private Slider _slider;

	void OnEnable()
	{
		_tick = StartCoroutine (Tick());
	}

	private IEnumerator Tick(){

		while (true) {
			yield return _seconds;
			_slider.IncrementStep ();
			Log (transform.parent.name);
		}
	}


	void OnDisable()
	{
		if (_tick != null)
			StopCoroutine (_tick);
	}

}
