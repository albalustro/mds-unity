using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousMove : MonoBehaviour
{

    [SerializeField]
    private float _movingSpeed;

    [SerializeField]
    private Vector3 _direction;

    [SerializeField]
    private float _maxDistance;
    private float _currentDistance;

    // Update is called once per frame
    void Update()
    {

        Vector3 displace = _direction * _movingSpeed * Time.deltaTime;
        _currentDistance += displace.magnitude;

        transform.position = transform.position + displace;

        if(_currentDistance >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public void OnMouseDown()
    {
        enabled = false;
    }

}
