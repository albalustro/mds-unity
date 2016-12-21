using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour {

	private Animator m_anim;

	void Awake()
	{
		if (GetComponent<Animator> () == null) {
			Debug.LogError ("Não existe Animator neste GameObject");
		} else {
			m_anim = GetComponent<Animator> ();
		}
	}


	public void IsWalking(int n)
	{
		string move = "IsMoving";

		if (n == 1) {
			m_anim.SetBool (move, true);
		} else {
			m_anim.SetBool (move, false);
		}
	}

}
