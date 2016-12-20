using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;
using MDS.Core.Interfaces;


public class ContinuousMove : MDSBehaviour
{

    [SerializeField]
    private float _movingSpeed;
	private float m_currentSpeed;

    [SerializeField]
    private Vector3 _direction;

    [SerializeField]
    private float _maxDistance;
    private float _currentDistance;

	private Vector2 m_startPosition;

	[SerializeField]
	private bool m_destroyOnMaxDistance;

	[SerializeField]
	[InspectorShowIf("m_changeHeight")]
	private bool m_changeHeight;

	[SerializeField]
	private float m_YOffSet;

	[SerializeField]
	private bool m_changeSpeed;

	[SerializeField]
	private float m_speedVariation;

	public bool m_freezeOnMaxDistance;
	private bool m_isFreezed = false;




	public IAction[] m_onMaxDistanceAction;


	void Start()
	{
		m_startPosition = transform.position;
		m_currentSpeed = _movingSpeed;
	}

    // Update is called once per frame
    void Update()
    {
		if (!m_isFreezed) {
		
			Vector3 displace = _direction * m_currentSpeed * Time.deltaTime;
			_currentDistance += displace.magnitude;

			transform.position = transform.position + displace;

			if (_currentDistance >= _maxDistance) {
				if (m_destroyOnMaxDistance)
					Destroy (gameObject);
				else {
					ResetPosition ();

				}
			}
		}
    }

	void ResetPosition()
	{

		ExecuteActions(m_onMaxDistanceAction);

		if (m_freezeOnMaxDistance) {
			m_isFreezed = true;
			return;
		}
		
		transform.position = m_startPosition;

		if (m_changeHeight) {
			float newHeight = Random.Range (m_startPosition.y - m_YOffSet, m_startPosition.y + m_YOffSet);
			transform.position = new Vector2 (transform.position.x, newHeight);
		}

		if (m_changeSpeed) {
			float newSpeed = Random.Range (_movingSpeed, _movingSpeed + m_speedVariation);
			m_currentSpeed = newSpeed;
		}

		_currentDistance = 0f;
	}

    public void OnMouseDown()
    {
        enabled = false;
    }

}
