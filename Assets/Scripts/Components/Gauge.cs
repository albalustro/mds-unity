using UnityEngine;
using System.Collections;
using System;

public class Gauge : MDSBehaviour {

    [SerializeField]
    private Transform _gaugePointer;

    [SerializeField]
    private GaugeValue _minimum;

    [SerializeField]
    private GaugeValue _maximum;

    private int _value;

	void Start () {
        _gaugePointer.localRotation = Quaternion.Euler(0, 0, _minimum.ZRotation);
        _value = _minimum.Value;
	}
	
	
    public void OnTickHandler(int curValue)
    {
        _value = curValue;
        RefreshPointerAngle(_value);

    }

    public void Increment()
    {
        _value++;
        _value = Mathf.Clamp(_value, _minimum.Value, _maximum.Value);
        RefreshPointerAngle(_value);
    }

    public void Decrement()
    {
        _value--;
        _value = Mathf.Clamp(_value, _minimum.Value, _maximum.Value);
        RefreshPointerAngle(_value);
    }

    private void RefreshPointerAngle(int curValue)
    {
        float curPercent = (float)(curValue - _minimum.Value) / (float)(_maximum.Value - _minimum.Value);
        float curRotation = _minimum.ZRotation + (_maximum.ZRotation - _minimum.ZRotation) * curPercent;
        _gaugePointer.localRotation = Quaternion.Euler(0, 0, curRotation);
    }

    [Serializable]
    public class GaugeValue
    {
        public int Value { get; set; }
        public float ZRotation { get; set; }
    }

}
