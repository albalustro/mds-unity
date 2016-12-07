using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MDSBehaviour {

    [SerializeField]
    private SpriteRenderer _highDigit;
    [SerializeField]
    private SpriteRenderer _lowDigit;

    private int _currentValue;

    [SerializeField]
    private Sprite[] _numbers;

    public void Start()
    {
        if(_numbers.Length != 10)
        {
            LogError("Erro no vetor de números no Counter");
        }

        _currentValue = 0;
        Refresh();
    }

    public void Increment()
    {
        _currentValue++;
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
