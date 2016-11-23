using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Clock : MDSBehaviour
{

    [SerializeField]
    private SpriteRenderer _tenMinuteDigit;
    [SerializeField]
    private SpriteRenderer _minuteDigit;

    [SerializeField]
    private SpriteRenderer _tenSecondDigit;
    [SerializeField]
    private SpriteRenderer _secondDigit;

    [SerializeField]
    private Sprite[] _numbers;

    public void Start()
    {
        if (_numbers.Length != 10)
        {
            Debug.LogError("Erro no vetor de números no Clock da cena <" + SceneManager.GetActiveScene().name + ">");
        }
    }

    /// <summary>
    /// Metodo a ser usado no evento de um timer
    /// </summary>
    public void OnTickHandler(int sec)
    {
        
        int minuts = sec / 60;
        int seconds = sec % 60;

        int high_minuts = minuts / 10;
        int low_minuts = minuts % 10;

        int high_seconds = seconds / 10;
        int low_seconds = seconds % 10;

        _tenMinuteDigit.sprite = _numbers[high_minuts];
        _minuteDigit.sprite = _numbers[low_minuts];

        _tenSecondDigit.sprite = _numbers[high_seconds];
        _secondDigit.sprite = _numbers[low_seconds];

    }

}
