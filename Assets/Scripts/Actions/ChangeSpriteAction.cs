using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;
using FullInspector;
using UnityEngine.UI;

namespace MDS.Actions{

	[SerializeField]
	public class ChangeSpriteAction : BaseAction {

		[SerializeField] private bool m_useMemorizedObj;

		[SerializeField, InspectorHideIf("m_useMemorizedObj")] private GameObject m_obj;
		[SerializeField] private Sprite m_newSprite;
		[SerializeField] private bool m_lockCollider;


		public override IEnumerator Execute()
		{
            if(byPass) yield break;
            yield return base.Execute();
			if (m_useMemorizedObj) {
				m_obj = MemorizeMe.MemorizedGameObject;
			} 

			if(m_obj.GetComponent<SpriteRenderer> () != null)
				m_obj.GetComponent<SpriteRenderer> ().sprite = m_newSprite;
			else if(m_obj.GetComponent<Image> () != null)
				m_obj.GetComponent<Image> ().sprite = m_newSprite;

			Collider2D col = m_obj.GetComponent<Collider2D> ();
			if (col != null) {
				if (m_lockCollider) {
					col.enabled = false;
				}
			}

		}


	}

}
