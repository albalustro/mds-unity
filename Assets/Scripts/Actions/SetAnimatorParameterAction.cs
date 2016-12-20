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
			Bool
		}

		[SerializeField] private Animator m_anim;
		[SerializeField] private AnimatorParameterAction m_animatorAction;
		[SerializeField] private string m_parameter;
		[SerializeField] private int m_value;
		[SerializeField] private bool m_boolValue;
		 

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
			case AnimatorParameterAction.Bool:
				m_anim.SetBool (m_parameter, m_boolValue);
				break;
			}

		}

	}

}
