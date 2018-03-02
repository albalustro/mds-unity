using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    [SerializeField]
    private Playmove.PYTextBox FieldToType;

    private int _position = 0;
    private int Position
    {
        get { return _position; }
        set { _position = (int)Mathf.Clamp(value, 0, Mathf.Infinity); }
    }

    private bool _withDisplay, _capsOn, _firstInput = true;

    private void Start()
    {
        Playmove.PlayTableKeyboard.Instance.onCancel.AddListener(Playmove.PlayTableKeyboard.Instance.Close);
        Playmove.PlayTableKeyboard.Instance.OnType += Keyboard_OnType;
    }

    private void Keyboard_OnType(Playmove.PKKeyButton key)
    {
        if (_withDisplay) return;

        if (_firstInput)
            FieldToType.Text = "";
        _firstInput = false;

        switch (key.Type)
        {
            case Playmove.PKKeyType.Letter:
                SetText(key.Text);
                break;
            case Playmove.PKKeyType.Space:
                SetText(" ");
                break;
            case Playmove.PKKeyType.Backspace:
                FieldToType.Text = FieldToType.Text.Remove((int)Mathf.Clamp(Position - 1, 0, Mathf.Infinity), 1);
                Position--;
                break;
            case Playmove.PKKeyType.CursorNavigationLeft:
                Position = (int)Mathf.Clamp(Position - 1, 0, Mathf.Infinity);
                break;
            case Playmove.PKKeyType.CursorNavigationRight:
                Position = Mathf.Clamp(Position + 1, 0, FieldToType.Text.Length);
                break;
            case Playmove.PKKeyType.ClearAllText:
                FieldToType.Text = "";
                Position = 0;
                break;
            case Playmove.PKKeyType.Point:
                SetText(".");
                break;
        }
    }

    private void SetText(string text)
    {
        FieldToType.Text = FieldToType.Text.Substring(0, Position) + text + FieldToType.Text.Substring(Position);
        Position++;
    }

    public void OpenKeyboard(bool displayActive)
    {
        _withDisplay = displayActive;
        Playmove.PlayTableKeyboard.Instance.DisableDefaultRules();
        Playmove.PlayTableKeyboard.Instance.Open(displayActive);
    }

    public void ActivateCaps()
    {
        if (_capsOn)
            Playmove.PlayTableKeyboard.Instance.DisableCapsLock();
        else
            Playmove.PlayTableKeyboard.Instance.EnableCapsLock();

        _capsOn = !_capsOn;
    }
}
