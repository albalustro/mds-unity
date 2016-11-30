using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Gameplay.Selectable;
using System;


namespace MDS.Actions {

	[SerializeField]
	public class FadeInOutAction : IAction {


		[SerializeField] private GameObject m_obj;
		[SerializeField] private bool m_fadeIn = false;
		private int m_fadeTime = 2;


		public IEnumerator Execute(Action callback = null)
		{
			if (m_fadeIn) {
				LeanTween.alpha (m_obj, 0, 0f);
				LeanTween.alpha (m_obj, 1, m_fadeTime);
			} else {
				LeanTween.alpha (m_obj, 1, 0f);
				LeanTween.alpha (m_obj, 0, m_fadeTime);
			}

			yield return null;
		}


	}
}
