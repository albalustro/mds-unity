using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;


namespace MDS.Actions {

	[SerializeField]
	public class FadeInOutAction : BaseAction
	{

        public bool selfTarget;

        [FullInspector.InspectorHideIf("selfTarget")]
        [SerializeField] private GameObject m_obj;
		[SerializeField] private bool m_fadeIn = false;
		[SerializeField] private float m_fadeTime = 2;


		public override IEnumerator Execute()
		{
			yield return base.Execute();

            if(selfTarget)
                m_obj = _corotineHolder.gameObject;

			SpriteRenderer sr = m_obj.GetComponent<SpriteRenderer>();
			Color c = sr.color;
			if(m_fadeIn) {
				c.a = 0;
				sr.color = c;
				LeanTween.alpha (m_obj, 1, m_fadeTime);
			} else {
				c.a = 1;
				sr.color = c;
				LeanTween.alpha (m_obj, 0, m_fadeTime);
			}

			yield return new WaitForSeconds(m_fadeTime);
		}


	}
}
