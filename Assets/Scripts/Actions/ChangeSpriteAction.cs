using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using System;

namespace MDS.Actions{

	[SerializeField]
	public class ChangeSpriteAction : BaseAction {

		[SerializeField] private GameObject m_obj;
		[SerializeField] private Sprite m_newSprite;
		[SerializeField] private bool m_lockCollider;

		public override IEnumerator Execute()
		{
            yield return base.Execute();

            m_obj.GetComponent<SpriteRenderer> ().sprite = m_newSprite;

			if (m_lockCollider) {
				m_obj.GetComponent<Collider2D> ().enabled = false;
			}

		}


	}

}
