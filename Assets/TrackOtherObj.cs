using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Gameplay.DragDrop;

public class TrackOtherObj : MonoBehaviour {

	[SerializeField]
	private string m_thisLabel;

	[SerializeField]
	private Sprite m_rightSprite, m_wrongSprite;
	private Sprite m_defaultSprite;

	[SerializeField]
	private DropGroupSlot m_slot;

	private Draggable m_draggable;

	private SpriteRenderer m_spriteRenderer;

	void Awake()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Start()
	{
		m_defaultSprite = m_spriteRenderer.sprite;
	}

	void Update()
	{
		m_draggable = m_slot._draggableReference;

		if (m_draggable != null) {
			if (m_draggable.Labels [0] == m_thisLabel) {
				m_spriteRenderer.sprite = m_rightSprite;
			} else {
				m_spriteRenderer.sprite = m_wrongSprite;
			}
		} else {
			m_spriteRenderer.sprite = m_defaultSprite;
		}

	}






}
