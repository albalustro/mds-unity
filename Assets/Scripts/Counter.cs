using UnityEngine;
using UnityEngine.SceneManagement;

public class Counter : MDSBehaviour
{

    [SerializeField]
    private SpriteRenderer _firstDigit;
    [SerializeField]
    private SpriteRenderer _secondDigit;
    [SerializeField]
    private Sprite[] _numbers;
	private int _amount;

    public void Start()
    {

//        if (_numbers.Length != 10)
//        {
//            Debug.LogError("Erro no vetor de números no Clock da cena <" + SceneManager.GetActiveScene().name + ">");
//        }
    }

    /// <summary>
    /// Metodo a ser usado no evento de um timer
    /// </summary>
    //public void OnTickHandler(int num)
    //{
    //    int high_first_digit = num / 10;
    //    int low_first_digit = num % 10;

    //    int high_second_digit = num / 10;
    //    int low_second_digit = num % 10;

    //    _firstDigit.sprite = _numbers[high_seconds];
    //    _firstDigit.sprite = _numbers[low_seconds];
    //}

}
