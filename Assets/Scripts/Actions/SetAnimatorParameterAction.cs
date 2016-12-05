using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using System;
using FullInspector;


namespace MDS.Actions{

	public class SetAnimatorParameterAction : IAction {

		public enum AnimatorParameterAction
		{
			trigger,
			integer,
		}

		[SerializeField] private Animator m_anim;
		[SerializeField] private AnimatorParameterAction m_animatorAction;
		[SerializeField] private string m_parameter;
		[SerializeField] private int m_value;

		public IEnumerator Execute(Action callback = null)
		{

			switch (m_animatorAction) {
				
			case AnimatorParameterAction.trigger:
				m_anim.SetTrigger (m_parameter);
				break;

			case AnimatorParameterAction.integer:
				if (m_value != null) {
					m_anim.SetInteger (m_parameter, m_value);
				}
				break;

			}

			yield return null;
		}

	}

}
