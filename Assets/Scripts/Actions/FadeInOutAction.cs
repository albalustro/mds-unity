using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;
using MDS.Gameplay.DragDrop;



namespace MDS.Actions {

	[SerializeField]
	public class FadeInOutAction : BaseAction
	{

        public bool selfTarget;

        [FullInspector.InspectorHideIf("selfTarget")]
		[SerializeField] private GameObject[] m_target;
		[SerializeField] private DropGroupSlot m_useDropSlotAsTarget;
		[SerializeField] private bool m_fadeIn = false;
		[SerializeField] private float m_fadeTime = 2;



		public override IEnumerator Execute()
		{
			yield return base.Execute();

			if (selfTarget) {
				m_target = new GameObject[1];
				m_target [0] = _corotineHolder.gameObject;
			}

			if (m_useDropSlotAsTarget) {
				m_target = new GameObject[1];
				m_target[0] = m_useDropSlotAsTarget.draggableReference.gameObject;
			}

			for (int i = 0; i < m_target.Length; i++) {
				SpriteRenderer sr = m_target[i].GetComponent<SpriteRenderer>();
				Color c = sr.color;
				if(m_fadeIn) {
					c.a = 0;
					sr.color = c;
					LeanTween.alpha (m_target[i], 1, m_fadeTime);
				} else {
					c.a = 1;
					sr.color = c;
					LeanTween.alpha (m_target[i], 0, m_fadeTime);
				}

			}

			yield return new WaitForSeconds(m_fadeTime);
		}


	}
}
