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

        public FadeInOutAction()
        {

        }

        public FadeInOutAction(bool fadein, float fadeTime, GameObject[] targets, bool wait, float delay)
        {
            m_fadeIn = fadein;
            m_fadeTime = fadeTime;
            m_target = targets;
            delayBeforeExecution = delay;
            waitFinish = wait;
        }

		public override IEnumerator Execute()
		{
            if(byPass) yield break;
            yield return base.Execute();

			if (selfTarget) {
				m_target = new GameObject[1];
				m_target [0] = _corotineHolder.gameObject;
			}

			if (m_useDropSlotAsTarget) {
				m_target = new GameObject[1];
				m_target[0] = m_useDropSlotAsTarget.draggableReference.gameObject;
			}

            float alphaDestination = m_fadeIn ? 1f : 0f;

			for (int i = 0; i < m_target.Length; i++) {
				SpriteRenderer sr = m_target[i].GetComponent<SpriteRenderer>();
                if(sr != null)
                {
                    Color c = sr.color;
                    c.a = 1f - alphaDestination;
                    sr.color = c;
                    LeanTween.alpha(m_target[i], alphaDestination, m_fadeTime);
                }
                else
                {
                    CanvasGroup canvasGroup = m_target[i].GetComponent<CanvasGroup>();
                    if(canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f - alphaDestination;
                        LeanTween.alphaCanvas(canvasGroup, alphaDestination, m_fadeTime);
                    }
                }


			}

			yield return new WaitForSeconds(m_fadeTime);
		}


	}
}
