using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using System;
using FullInspector;


namespace MDS.Actions{

	public class SetAnimatorParameterAction : BaseAction
	{

		public enum AnimatorParameterAction
		{
			Trigger,
			Integer,
		}

		[SerializeField] private Animator m_anim;
		[SerializeField] private AnimatorParameterAction m_animatorAction;
		[SerializeField] private string m_parameter;
		[SerializeField] private int m_value;

		public override IEnumerator Execute()
		{
            yield return base.Execute();


            switch(m_animatorAction) {
				
			case AnimatorParameterAction.Trigger:
				m_anim.SetTrigger (m_parameter);
				break;

			case AnimatorParameterAction.Integer:
				m_anim.SetInteger (m_parameter, m_value);
				break;

			}

		}

	}

}
