using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecrementVisualCounter : MDSBehaviour {


	[SerializeField]
	private GameObject[] m_target;

	private int m_currentIndex = 0;

	public void DecrementCounter()
	{
		if (m_currentIndex >= m_target.Length)
			return;
		
		GameObject tmpGo = m_target [m_currentIndex];
		SpriteRenderer sr = tmpGo.GetComponent<SpriteRenderer>();
		Color c = sr.color;

		c.a = 1;
		sr.color = c;

		LeanTween.alpha (tmpGo, 0, 2f);

		++m_currentIndex;

	}

}
