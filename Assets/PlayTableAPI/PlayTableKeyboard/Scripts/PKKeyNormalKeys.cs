using UnityEngine;

namespace Playmove
{
    public class PKKeyNormalKeys : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] Lines;

        [SerializeField]
        private PKKeyButton Confirm;

        [SerializeField]
        private PKKeyButton Cancel;

        [SerializeField]
        private PKKeyButton Backspace;

        [SerializeField]
        private PKKeyButton ClearAll;

        [SerializeField]
        private PKKeyButton SpecialKey;

        public int Page;

        public bool CapsLock = true;

        private TextAsset _keyboardConfiguration { get { return Resources.Load<TextAsset>(string.Format("Localization/{0}/KeyboardConfiguration", PlaytableWin32.Instance.Language)); } }

        private PKKeyCapsLock CapsLockBtn { get { return GetComponentInChildren<PKKeyCapsLock>(true); } }

        private int _nextPage;

        // Linhas x Letra
        private PKKeyButton[][] Letters;

        private PKKeyboardLettersLoader LettersLoader;

        public string ConfirmDefaultText { get { return LettersLoader.Confirm; } }
        public string CancelDefaultText { get { return LettersLoader.Cancel; } }

        private void Awake()
        {
            LettersLoader = new PKKeyboardLettersLoader(_keyboardConfiguration);

            //Set Special Keys
            Confirm.Text = LettersLoader.Confirm;
            Cancel.Text = LettersLoader.Cancel;
            Backspace.Text = LettersLoader.Backspace;
            ClearAll.Text = LettersLoader.ClearAll;

            SpecialKey.Text = LettersLoader.Pages[1].SpecialLetter;

            //Getting normal keys component reference
            Letters = new PKKeyButton[Lines.Length][];
            for (int i = 0; i < Letters.Length; i++)
            {
                PKKeyButton[] lettersComponet = Lines[i].GetComponentsInChildren<PKKeyButton>();
                Letters[i] = new PKKeyButton[lettersComponet.Length];
                for (int j = 0; j < lettersComponet.Length; j++)
                {
                    Letters[i][j] = lettersComponet[j];
                }
            }

            SetPage(0, 1);
        }

        public void SetActiveCapsLockBtn(bool active)
        {
            CapsLock = true;
            CapsLockBtn.gameObject.SetActive(active);
        }

        public void SetCapsLock()
        {
            CapsLock = !CapsLock;
            SetPage(Page, _nextPage);
        }

        public void SetPage(int page, int nextPage)
        {
            Page = page;
            _nextPage = nextPage;

            for (int line = 0; line < Letters.Length; line++)
            {
                for (int letter = 0; letter < Letters[line].Length; letter++)
                {
                    if (CapsLock)
                        Letters[line][letter].SetKey(LettersLoader.Getletter(page, line, letter).ToUpper(), page);
                    else
                        Letters[line][letter].SetKey(LettersLoader.Getletter(page, line, letter).ToLower(), page);
                }
            }

            SpecialKey.SetKey(LettersLoader.Pages[nextPage].SpecialLetter, page);
        }
    }
}