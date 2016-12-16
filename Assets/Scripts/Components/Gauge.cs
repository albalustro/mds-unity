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



	void Start () {
        _gaugePointer.localRotation = Quaternion.Euler(0, 0, _minimum.ZRotation);
	}
	
	
    public void OnTickHandler(int curValue)
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
