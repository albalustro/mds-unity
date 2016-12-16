using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MDSBehaviour
{

    [SerializeField]
    private SpriteRenderer _highDigit;
    [SerializeField]
    private SpriteRenderer _lowDigit;

	private int _currentValue;
	private int _startValue;

	public int StartValue{
		set { _startValue = value; }
	}

    [SerializeField]
    private Sprite[] _numbers;

    public void Start()
    {
        if(_numbers.Length != 10)
        {
            LogError("Erro no vetor de números no Counter");
        }

		_currentValue = _startValue;
        Refresh();
    }

	public void Reset(int num)
	{
		_startValue = num;
		_currentValue = num;
		Refresh ();
	}

    public void Increment()
    {
        _currentValue++;
        Refresh();
    }

	public void Decrement()
	{
		if (_currentValue == 0)
		{
			LogError ("Counter decrementando valor nulo.");
			return;
		}
		_currentValue--;
		Refresh();
	}

    private void Refresh()
    {
        int low = _currentValue % 10;
        int high = _currentValue / 10;

        _lowDigit.sprite = _numbers[low];
        _highDigit.sprite = _numbers[high];
    }
}
