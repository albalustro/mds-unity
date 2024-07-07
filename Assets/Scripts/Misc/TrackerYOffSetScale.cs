using System;
using FullInspector;
using UnityEngine;

public class TrackerYOffSetScale : MDSBehaviour {

    [Serializable]
    public class YScale
    {
        public float y;
        public Vector3 scale;
    }

    [SerializeField, FullInspector.InspectorOrder(0)]
    private YScale _minYScale;

#if UNITY_EDITOR
    [InspectorButton, FullInspector.InspectorOrder(1)]
    public void CaptureMin()
    {
        _minYScale = new YScale();
        _minYScale.y = transform.position.y;
        _minYScale.scale = transform.localScale;
    }
#endif

    [SerializeField, FullInspector.InspectorOrder(2)]
    private YScale _maxYScale;

#if UNITY_EDITOR
    [InspectorButton, FullInspector.InspectorOrder(3)]
    public void CaptureMax()
    {
        _maxYScale = new YScale();
        _maxYScale.y = transform.position.y;
        _maxYScale.scale = transform.localScale;
    }
#endif

    float signal;

    private void Start()
    {
        signal = Mathf.Sign(transform.localScale.x);
    }

    void Update()
	{
        float curY = transform.position.y;
        float t = (curY - _minYScale.y) / (_maxYScale.y - _minYScale.y);
        Vector3 scale = Vector3.Lerp(_minYScale.scale, _maxYScale.scale, t);
        scale.x = Mathf.Abs(scale.x) * signal;
        transform.localScale = scale;
	}

}
