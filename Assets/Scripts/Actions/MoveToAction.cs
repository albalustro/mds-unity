using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Actions;
using MDS.Gameplay.DragDrop;


public class MoveToAction : BaseAction {


	[SerializeField]
	private bool m_selfTarget;

	[SerializeField]
	private GameObject m_obj;

	[SerializeField]
	private Transform m_destination;

	[SerializeField]
	private float m_duration;

	[SerializeField]
	private LeanTweenType easeType;

	private DropGroupSlot m_destinationSlot;

	private GameObject tmpGO;

	private bool _animationComplete;

	[SerializeField]
	private bool m_useMemorizedGo;


	public override IEnumerator Execute()
	{
		yield return base.Execute();

		if (m_selfTarget)
			tmpGO = _corotineHolder.gameObject;
		else {
			tmpGO = m_obj;
		}

		if (m_useMemorizedGo) {
			tmpGO = MemorizeMe.MemorizedGameObject;
		}

		LeanTween.move (tmpGO, m_destination.position, m_duration);
//		SetInSlot ();

	}


	void SetInSlot()
	{
		BaseDropGroupArea group = m_destination.GetComponent<BaseDropGroupArea> ();
		DropGroupSlot slot = null;
		Draggable drag = tmpGO.GetComponent<Draggable> ();
		group.SetInSlot (drag, ref slot);

	}

}
