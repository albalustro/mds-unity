using UnityEngine;

namespace Playmove
{
    [RequireComponent(typeof(PlayTableKeyboard))]
    public class PKEnableButtons : MonoBehaviour
    {
        private PlayTableKeyboard Keyboard { get { return GetComponent<PlayTableKeyboard>(); } }

        public const string BLOCK_KEYBOARD_CHARS = "',.-";

        public void EnableButtons(PKKeyButton[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                switch (keys[i].Type)
                {
                    case PKKeyType.Confirm:
                        Keyboard.DisableKey(keys[i], Keyboard.Text.Length <= 0 || !char.IsLetter(Keyboard.Text[0]));
                        break;

                    case PKKeyType.Space:
                        Keyboard.DisableKey(keys[i], (Keyboard.CursorPosition == 0 ||
                            (Keyboard.Text.Length > 0 && Keyboard.CursorPosition > 0 && Keyboard.Text[Keyboard.CursorPosition - 1] == ' ') ||
                            (Keyboard.Text.Length > Keyboard.CursorPosition && Keyboard.Text[Keyboard.CursorPosition] == ' ' && Keyboard.CursorPosition != Keyboard.Text.Length - 1)
                            ));
                        break;

                    case PKKeyType.CursorNavigationLeft:
                        Keyboard.DisableKey(keys[i], Keyboard.CursorPosition == 0);
                        break;

                    case PKKeyType.Backspace:
                        Keyboard.DisableKey(keys[i], Keyboard.CursorPosition == 0 || Keyboard.Text.Length == 0);
                        break;

                    case PKKeyType.CursorNavigationRight:
                        Keyboard.DisableKey(keys[i], Keyboard.CursorPosition >= Keyboard.Text.Length);
                        break;

                    case PKKeyType.ClearAllText:
                        Keyboard.DisableKey(keys[i], Keyboard.Text.Length <= 0);
                        break;

                    case PKKeyType.Letter:
                        keys[i].ChangeState(PYButtonState.Idle);
                        for (int j = 0; j < BLOCK_KEYBOARD_CHARS.Length; j++) // Não poderá colocar carcaters que não são letras como primeira letra
                        {
                            if (keys[i].Text.Contains(BLOCK_KEYBOARD_CHARS[j].ToString()))
                                Keyboard.DisableKey(keys[i], Keyboard.CursorPosition == 0);
                        }

                        if (string.IsNullOrEmpty(keys[i].Text))
                            Keyboard.DisableKey(keys[i], true);
                        break;
                }
            }
        }

        public void DisableUnusedKeys(PKKeyButton[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Keyboard.DisableKey(keys[i], string.IsNullOrEmpty(keys[i].Text));
            }
        }
    }
}