using UnityEngine;

public class TriggerObject : MonoBehaviour {

	[SerializeField]
	private Animator m_anim;

	[SerializeField]
	private string m_parameter;

	void OnMouseDown()
	{
		m_anim.SetTrigger (m_parameter);
	}

}
