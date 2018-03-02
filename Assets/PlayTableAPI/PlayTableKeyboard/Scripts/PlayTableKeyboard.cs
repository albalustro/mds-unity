using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Playmove
{
    public enum KeyboardState
    {
        Closed, Open, Text, Custom
    }

    public class PlayTableKeyboard : MonoBehaviour
    {
        #region Singleton

        private static PlayTableKeyboard instance;

        public static PlayTableKeyboard Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayTableKeyboard>();
                    if (instance == null)
                        instance = Instantiate(Resources.Load<PlayTableKeyboard>("PlaytableKeyboardNew"));
                }
                return instance;
            }
        }

        #endregion Singleton

        [Header("PlaytableKeyboard")]
        [SerializeField]
        private float TimeTickPressed = 0.25f;

        public KeyboardState CurrentState = KeyboardState.Closed;

        /// <summary>
        /// Page 1: Normal Chars
        /// Page 2: Special Chars
        /// Page 3: Numbers
        /// </summary>
        [SerializeField]
        private int _totalPages = 2;

        [SerializeField]
        private bool _activeCapsLock = false;

        #region Properties

        private string _text = "";

        public string Text
        {
            get { return _text; }
            private set
            {
                _text = value;
                if (SetTextDelegates != null)
                    SetTextDelegates(_text);
            }
        }

        private int cursorPosition = 0;

        /// <summary>
        /// Type position
        /// </summary>
        public int CursorPosition
        {
            get { return cursorPosition; }
            private set
            {
                cursorPosition = value;
                if (Display.isActiveAndEnabled)
                    cursorPosition = Mathf.Clamp(value, 0, Display.MaxChars - 1);
                if (OnCursorChangedPosition != null)
                    OnCursorChangedPosition.Invoke(cursorPosition);
            }
        }

        [SerializeField]
        private PKKeyboardSkin skin;

        public PKKeyboardSkin Skin
        {
            get
            {
                return skin;
            }
            set
            {
                skin = value;
            }
        }

        private bool _mute;

        public bool Mute
        {
            get
            {
                return _mute;
            }
            set
            {
                _mute = value;
                if (_voiceSource != null)
                    _voiceSource.mute = _mute;
            }
        }

        private bool IsNormalPage { get { return _pageIndex == 0; } }

        private int NextSpecialPage
        {
            get
            {
                int page = _pageIndex + 1;
                if (page >= _totalPages)
                    page = 0;
                return page;
            }
        }

        #endregion Properties

        #region Componets

        private PKKeyboardDisplay Display { get { return GetComponentInChildren<PKKeyboardDisplay>(true); } }

        private Camera _cameraRef;

        private Camera CameraRef
        {
            get
            {
                if (!_cameraRef)
                    _cameraRef = Camera.main;
                return _cameraRef;
            }
            set { _cameraRef = value; }
        }

        private PKEnableButtons _enableRules;

        private PKEnableButtons EnableRules
        {
            get
            {
                if (!_enableRules)
                {
                    _enableRules = GetComponent<PKEnableButtons>();
                    if (!_enableRules)
                        _enableRules = gameObject.AddComponent<PKEnableButtons>();
                }
                return _enableRules;
            }
        }

        private PKKeyNormalKeys NormalKeys { get { return GetComponentInChildren<PKKeyNormalKeys>(); } }

        #endregion Componets

        #region Events

        [Serializable]
        public class KeyboardTextEventCursorPosition : UnityEvent<int> { }

        public KeyboardTextEventCursorPosition OnCursorChangedPosition = new KeyboardTextEventCursorPosition();

        public event Action<PKKeyButton> OnType;

        [Serializable]
        public class KeyboardEvent : UnityEvent { }

        [Serializable]
        public class KeyboardTextEvent : UnityEvent<string> { }

        [HideInInspector]
        public Camera Camera;

        [Header("Events")]
        public KeyboardEvent onKeyboardOpen = new KeyboardEvent();

        public KeyboardEvent onKeyboardClose = new KeyboardEvent();
        public KeyboardEvent onShowText = new KeyboardEvent();
        public KeyboardEvent onInvalidName = new KeyboardEvent();
        public KeyboardEvent onCancel = new KeyboardEvent();
        public KeyboardTextEvent onConfirm = new KeyboardTextEvent();

        public KeyboardTextEvent OnKeyDown = new KeyboardTextEvent();
        public KeyboardTextEvent OnKeyUp = new KeyboardTextEvent();
        public KeyboardTextEvent OnTextChange = new KeyboardTextEvent();
        public KeyboardTextEvent OnCursorMoved = new KeyboardTextEvent();

        #endregion Events

        private AudioSource _voiceSource;
        private bool _isEnabled = true;
        private bool _hasAutomaticPressed = false;
        private bool _useDefaultRules = true;
        private PKKeyButton[] _keys;
        private PKKeyButton _pressedKey;
        private PKKeyButton _cancelBtn;
        private PKKeyButton _confirmBtn;
        private PKKeyButton _muteKey;
        private PYTweenAnimation _tween;
        private const string BLOCK_KEYBOARD_CHARS = "',.-";
        private int _pageIndex = 0;

        public delegate void SetTextDelegate(string text);

        public SetTextDelegate SetTextDelegates;

        #region Unity

        private void Awake()
        {
            if (Camera == null)
                Camera = Camera.main;
            Initialize();
        }

        private void Initialize()
        {
            BannedWordsManager.LoadBannedWords();

            _keys = GetComponentsInChildren<PKKeyButton>(true);
            _tween = PYTweenAnimation.Add(gameObject).SetDuration(0.75f).SetEaseType(Ease.Type.InOutExpo);

            if (_voiceSource == null)
            {
                GameObject o = new GameObject("VoiceSource");
                o.transform.parent = transform;
                _voiceSource = o.AddComponent<AudioSource>();
                _voiceSource.playOnAwake = false;
            }

            foreach (PKKeyButton key in _keys)
            {
                if (key.Type == PKKeyType.Cancel)
                    _cancelBtn = key;
                if (key.Type == PKKeyType.Confirm)
                    _confirmBtn = key;

                key.onDown.AddListener(TouchManagerOnClick);
                key.onClick.AddListener((sender) =>
                {
                    OnRelasedKeyButton(sender, sender.IsPointerInside);
                });

                if (key.Type == PKKeyType.Mute)
                    _muteKey = key;
            }

            CheckMuteState();
            ChangeState(CurrentState);

            NormalKeys.SetActiveCapsLockBtn(_activeCapsLock);
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.O))
                Open();
            if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.N))
                ShowText();
            if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.C))
                Close();

            if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.Q))
                EnableNumbers();
            if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.W))
                DisableNumbers();

            if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.U))
            {
                _activeCapsLock = !_activeCapsLock;
                NormalKeys.SetActiveCapsLockBtn(_activeCapsLock);
            }
        }

#endif

        #endregion Unity

        /// <summary>
        /// Abrir o teclado com o display no topo ou não
        /// </summary>
        /// <param name="displayActive"></param>
        public void Open(bool displayActive = true)
        {
            SetCancelButtonText();
            Display.gameObject.SetActive(displayActive);
            ChangeState(KeyboardState.Open);
        }

        /// <summary>
        /// Abrir o teclado na posição x qualquer e se o display estará ativo
        /// </summary>
        /// <param name="xPosition">Posição global do teclado</param>
        /// <param name="displayActive"></param>
        public void Open(float xPosition, bool displayActive = true)
        {
            Display.gameObject.SetActive(displayActive);
            ChangeState(KeyboardState.Custom, xPosition);
        }

        public void Open(UnityAction cancelAction)
        {
            onCancel.AddListener(cancelAction);
            Open();
        }

        public void Open(UnityAction cancelAction, float xPostion)
        {
            onCancel.AddListener(cancelAction);
            Open(xPostion);
        }

        public void SetCameraRef(Camera cam)
        {
            CameraRef = cam;
        }

        public void Close()
        {
            KeyActionSpecial(0);
            ChangeState(KeyboardState.Closed);
        }

        public void ShowText(string text = null)
        {
            if (!Display.isActiveAndEnabled) return;

            if (!string.IsNullOrEmpty(text))
                Text = text;
            ChangeState(KeyboardState.Text);
        }

        public void SetCursorPosition(int cursorPosition)
        {
            CursorPosition = cursorPosition;
        }

        public void SetText(string text)
        {
            Text = text;
            CursorPosition = text.Length;
            VerifyRulesAndEnableButtons();

            if (Display.isActiveAndEnabled)
                Display.UpdateCursorPosition(text.Length + 1);

            if (OnTextChange != null)
                OnTextChange.Invoke(Text);
            //OnTextChange(Text);
        }

        public void ClearText()
        {
            Text = "";
            CursorPosition = 0;

            if (OnTextChange != null)
                OnTextChange.Invoke(Text);
            //OnTextChange(Text);

            if (Display.isActiveAndEnabled)
                Display.UpdateCursorPosition(1);
        }

        public void VerifyRulesAndEnableButtons()
        {
            if (_useDefaultRules)
                EnableRules.EnableButtons(_keys);
            else
                EnableRules.DisableUnusedKeys(_keys);
        }

        public void SetCancelButtonText(string text = null)
        {
            if (string.IsNullOrEmpty(text))
                _cancelBtn.Text = NormalKeys.CancelDefaultText;
            else
                _cancelBtn.Text = text;
        }

        public void SetConfirmButtonText(string text)
        {
            if (string.IsNullOrEmpty(text))
                _confirmBtn.Text = NormalKeys.ConfirmDefaultText;
            else
                _confirmBtn.Text = text;
        }

        public void EnableNumbers()
        {
            _totalPages = 3;
        }

        public void DisableNumbers()
        {
            _totalPages = 2;
        }

        public void EnableCapsLock()
        {
            _activeCapsLock = true;
            NormalKeys.SetActiveCapsLockBtn(true);
        }

        public void DisableCapsLock()
        {
            _activeCapsLock = false;
            NormalKeys.SetActiveCapsLockBtn(false);
        }

        public void EnableDefaultRules()
        {
            _useDefaultRules = true;
            VerifyRulesAndEnableButtons();
        }

        public void DisableDefaultRules()
        {
            _useDefaultRules = false;
            EnableAllButtons();
        }

        public void DisableKey(PKKeyButton key, bool conditionToDisable)
        {
            if (conditionToDisable)
            {
                key.ChangeState(PYButtonState.Disabled);

                if (_pressedKey == key)
                    _pressedKey = null;
            }
            else
                key.ChangeState(PYButtonState.Idle);
        }

        #region privates

        private void Show(bool visible = true)
        {
            gameObject.SetActive(visible);
        }

        private void Hide()
        {
            Show(false);
        }

        private void ChangeState(KeyboardState state, float? pos = null)
        {
            if (CurrentState == state) return;

            NormalKeys.SetPage(0, NextSpecialPage);

            Vector3 fromPosition = Vector3.zero;
            Vector3 toPosition = Vector3.zero;
            Action callback = null;

            _isEnabled = false;

            bool inverted = ((int)CameraRef.transform.rotation.eulerAngles.z) == 180;

            switch (state)
            {
                case KeyboardState.Closed:
                default:
                    fromPosition = transform.position;
                    toPosition = transform.position + transform.up * -5;
                    callback = () =>
                    {
                        _isEnabled = true;
                        if (Display.isActiveAndEnabled)
                        {
                            Display.ResetDisplayPosition();
                            Text = "";
                        }
                    };
                    break;

                case KeyboardState.Open:
                    Show();
                    fromPosition = CameraRef.ViewportToWorldPoint(new Vector3(0.5f, inverted ? 1 : 0, 7.5f)) + transform.up * -5;
                    toPosition = CameraRef.ViewportToWorldPoint(new Vector3(0.5f, inverted ? 1 : 0, 7.5f));
                    if (Display.isActiveAndEnabled)
                        Display.ReturnText();
                    callback = () => { _isEnabled = true; };
                    break;

                case KeyboardState.Text:
                    Show();
                    fromPosition = transform.position;
                    toPosition = CameraRef.ViewportToWorldPoint(new Vector3(0.5f, inverted ? 1.29f : -0.29f, 7.5f));
                    if (Display.isActiveAndEnabled)
                        Display.CenterText();
                    break;

                case KeyboardState.Custom:
                    Show();
                    if (Display.isActiveAndEnabled)
                        Display.ReturnText();
                    callback = () => { _isEnabled = true; };
                    if (CurrentState == KeyboardState.Open)
                    {
                        fromPosition = transform.position;
                        toPosition = transform.position;
                        toPosition.x = pos.GetValueOrDefault();
                    }
                    else
                    {
                        fromPosition = CameraRef.ViewportToWorldPoint(new Vector3(0.5f, inverted ? 1 : 0, 7.5f)) + transform.up * -5;
                        fromPosition.x = pos.GetValueOrDefault();
                        toPosition = CameraRef.ViewportToWorldPoint(new Vector3(0.5f, inverted ? 1 : 0, 7.5f));
                        toPosition.x = pos.GetValueOrDefault();
                    }
                    state = KeyboardState.Open;
                    break;
            }

            CurrentState = state;

            _tween.Stop();
            _tween.SetPosition(fromPosition, toPosition);
            _tween.Play(callback);

            VerifyRulesAndEnableButtons();
        }

        private void TouchManagerOnClick(PYButton sender)
        {
            if (!_isEnabled)
                return;

            for (int i = 0; i < _keys.Length; i++) // Identify wich key was clicked
            {
                if (sender.OwnGameObject == _keys[i].OwnGameObject &&
                    _keys[i].State != PYButtonState.Disabled)
                {
                    _pressedKey = _keys[i];
                    _pressedKey.ChangeState(PYButtonState.Pressed);
                    if (OnKeyDown != null)
                        OnKeyDown.Invoke(_keys[i].Text);
                    //OnKeyDown(_keys[i].Text);

                    break;
                }
            }

            if (_pressedKey == null) // If clicked key is null, do nothing
                return;

            PKKeyButton keyButton = (sender != null) ? (PKKeyButton)sender : null;

            if ((keyButton != null) &&
                ((sender.OwnGameObject == _pressedKey.OwnGameObject)))
            {
                PlayKeySound();
                PlayKeyVoice();

                if (_pressedKey != null && OnType != null)
                    OnType(_pressedKey);

                switch (_pressedKey.Type)
                {
                    case PKKeyType.Letter:
                        if (Display.isActiveAndEnabled && Text.Length >= Display.MaxChars) break;
                        if (CursorPosition < Text.Length)
                        {
                            Text = Text.Substring(0, CursorPosition) + _pressedKey.Text + Text.Substring(CursorPosition);
                            //ChangeDisplayCursorPosition(1);
                            if (OnTextChange != null)
                                OnTextChange.Invoke(Text);
                            //OnTextChange(Text);
                        }
                        else
                        {
                            Text += _pressedKey.Text;
                            if (OnTextChange != null)
                                OnTextChange.Invoke(Text);
                            //OnTextChange(Text);
                        }

                        //if (CursorPosition >= Text.Length - 1 /*&& (Display.isActiveAndEnabled && CursorPosition < Display.MaxChars)*/)
                        ChangeDisplayCursorPosition(1);
                        //else
                        //    ChangeDisplayCursorPosition(0);

                        //if (!IsNormalPage) //voltar para a pagina normal autamaticamente
                        //    KeyActionSpecial(0);

                        break;

                    case PKKeyType.Space:
                        KeyActionSpace();

                        break;

                    case PKKeyType.Backspace:
                        if (!_hasAutomaticPressed)
                            KeyActionBackspace();
                        _hasAutomaticPressed = false;

                        break;

                    case PKKeyType.Confirm:
                        KeyActionConfirm();
                        break;

                    case PKKeyType.Cancel:
                        KeyActionCancel();
                        break;

                    case PKKeyType.Special:
                        KeyActionSpecial();
                        break;

                    case PKKeyType.CursorNavigationLeft:
                        if (!_hasAutomaticPressed)
                            KeyActionCursorNavigation(-1);
                        _hasAutomaticPressed = false;
                        break;

                    case PKKeyType.CursorNavigationRight:
                        if (!_hasAutomaticPressed)
                            KeyActionCursorNavigation(1);
                        _hasAutomaticPressed = false;
                        break;

                    case PKKeyType.ClearAllText:
                        ClearText();
                        break;

                    case PKKeyType.Mute:
                        Mute = !Mute;
                        break;

                    case PKKeyType.Point:
                        if (CursorPosition < Text.Length)
                        {
                            StringBuilder sb = new StringBuilder(Text + "");
                            sb[CursorPosition] = ".".ToCharArray()[0];
                            Text = sb.ToString();
                            if (OnTextChange != null)
                                OnTextChange.Invoke(Text);
                            //OnTextChange(Text);
                        }
                        else
                        {
                            Text += ".";
                            if (OnTextChange != null)
                                OnTextChange.Invoke(Text);
                            //OnTextChange(Text);
                        }

                        if (CursorPosition >= Text.Length - 1 && CursorPosition < Display.MaxChars)
                            ChangeDisplayCursorPosition(1);
                        else
                            ChangeDisplayCursorPosition(0);

                        if (!IsNormalPage) //Back to normal page automaticaly
                        {
                            KeyActionSpecial();
                        }

                        break;
                }
            }

            // If user keep holding the key, we execute the action automatic
            // based in the timeTick
            if (_pressedKey != null)
            {
                if (_pressedKey.Type == PKKeyType.Backspace)
                {
                    if (!IsInvoking("KeyActionBackspace"))
                        InvokeRepeating("KeyActionBackspace", TimeTickPressed, TimeTickPressed);
                }
                else if (_pressedKey.Type == PKKeyType.CursorNavigationLeft)
                {
                    if (!IsInvoking("KeyActionCursorNavigationLeft"))
                        InvokeRepeating("KeyActionCursorNavigationLeft", TimeTickPressed, TimeTickPressed);
                }
                else if (_pressedKey.Type == PKKeyType.CursorNavigationRight)
                {
                    if (!IsInvoking("KeyActionCursorNavigationRight"))
                        InvokeRepeating("KeyActionCursorNavigationRight", TimeTickPressed, TimeTickPressed);
                }
            }
        }

        private void OnRelasedKeyButton(PYButton sender, bool isMouseOver)
        {
            CancelDefaultInvokes();
            VerifyRulesAndEnableButtons();

            // If user clicked in a disabled key
            // the pressedKey var will be null, so just return
            if (_pressedKey == null)
                return;

            PKKeyButton keyButton = (sender != null) ? (PKKeyButton)sender : null;

            if ((keyButton != null) &&
                ((isMouseOver && sender == _pressedKey.OwnGameObject)))
            {
                if (OnKeyUp != null)
                    OnKeyUp.Invoke(_pressedKey.Text);
                //OnKeyUp(_pressedKey.Text);
            }

            _pressedKey.ChangeState(PYButtonState.Idle);
            _pressedKey = null;

            VerifyRulesAndEnableButtons();
            CheckMuteState();
        }

        private void KeyActionSpace()
        {
            if (Display.isActiveAndEnabled && Text.Length >= Display.MaxChars) return;
            if (CursorPosition == 0) return;

            Text = Text.Insert(CursorPosition, " ");

            if (OnTextChange != null)
                OnTextChange.Invoke(Text);

            ChangeDisplayCursorPosition(1);
        }

        private void KeyActionBackspace()
        {
            if (!_hasAutomaticPressed)
                PlayKeyVoice(false);

            _hasAutomaticPressed = true;

            PlayKeySound();

            if (Text.Length > 0)
            {
                Text = Text.Remove(CursorPosition - 1, 1);

                if (OnTextChange != null)
                    OnTextChange.Invoke(Text);

                ChangeDisplayCursorPosition(-1);
            }
        }

        private void KeyActionConfirm()
        {
            if ((Text.Length <= 0 || /*!char.IsLetter(Text[0])) ||*/ BannedName()))
            {
                InvalidName();
                return;
            }

            Text = Text.Trim();
            if (onConfirm != null)
                onConfirm.Invoke(Text);
        }

        private void CheckMuteState()
        {
            if (Mute)
                _muteKey.ChangeState(PYButtonState.Idle);
            else
                _muteKey.ChangeState(PYButtonState.Pressed);
        }

        private void InvalidName()
        {
            if (onInvalidName != null)
                onInvalidName.Invoke();
        }

        private bool BannedName()
        {
            foreach (string word in Text.Split(new char[] { ' ', ',', '.', '-', '\'' }))
            {
                foreach (string badWord in BannedWordsManager.BannedWords.Words)
                {
                    if (word.ToUpper().Trim() == badWord.ToUpper().Trim())
                    {
                        PYAudioManager.Instance.StartAudio(PYAudioTags.B_InvalidNameMsg, PYGroupTag.Voice).Play();
                        return true;
                    }
                }
            }

            return false;
        }

        private void KeyActionCancel()
        {
            if (onCancel != null)
                onCancel.Invoke();
            else
                Debug.LogWarning("There nothing assigned on Keyboard-Cancel-Button, please assign manualy!");
        }

        private void KeyActionSpecial(int? pageIndex = null)
        {
            _pageIndex++;
            if (pageIndex.HasValue)
                _pageIndex = pageIndex.Value;
            if (_pageIndex >= _totalPages)
                _pageIndex = 0;

            NormalKeys.SetPage(_pageIndex, NextSpecialPage);
        }

        private void KeyActionCursorNavigationLeft()
        {
            KeyActionCursorNavigation(-1);
        }

        private void KeyActionCursorNavigationRight()
        {
            KeyActionCursorNavigation(1);
        }

        private void KeyActionCursorNavigation(int direction)
        {
            if (!_hasAutomaticPressed)
                PlayKeyVoice(false);

            _hasAutomaticPressed = true;

            PlayKeySound();

            if (CursorPosition > 0 || Text.Length > 0)
            {
                ChangeDisplayCursorPosition(direction);
            }
        }

        private void SetPage(int page = 0)
        {
            GetComponent<PKKeyNormalKeys>().SetPage(page, NextSpecialPage);
        }

        private void ChangeDisplayCursorPosition(int direction)
        {
            CursorPosition += direction;

            if (Display.isActiveAndEnabled)
                Display.UpdateCursorPosition(CursorPosition + 1);

            VerifyRulesAndEnableButtons();
        }

        private void PlayKeySound()
        {
            if (_pressedKey != null && _pressedKey.CustomSkin.KeySound != null &&
                PYAudioManager.Instance != null && !PYAudioManager.Instance.IsMute(PYGroupTag.SFX))
                GetComponent<AudioSource>().PlayOneShot(_pressedKey.CustomSkin.KeySound);
        }

        private void PlayKeyVoice(bool interrupt = true)
        {
            if (_pressedKey != null && _pressedKey.KeyVoice != null &&
                PYAudioManager.Instance != null && !PYAudioManager.Instance.IsMute(PYGroupTag.Voice))
            {
                if (_voiceSource.clip != _pressedKey.KeyVoice)
                {
                    _voiceSource.clip = _pressedKey.KeyVoice;
                    interrupt = true;
                }

                if (!interrupt && _voiceSource.isPlaying)
                    return;

                PYAudioManager.Instance.StopGroup(PYGroupTag.Voice);
                _voiceSource.Play();
            }
        }

        private string FilterString(string text, bool changeCursorPosition = true)
        {
            if (text.Length == 0)
                return text;

            string filter = text.Replace("  ", " ");
            filter = filter[0] == ' ' ? filter.Remove(0, 1) : filter;
            return filter;
        }

        private void CancelDefaultInvokes()
        {
            if (IsInvoking("ShowAlternativeKeysForPressedKey"))
                CancelInvoke("ShowAlternativeKeysForPressedKey");

            if (IsInvoking("KeyActionBackspace"))
                CancelInvoke("KeyActionBackspace");

            if (IsInvoking("KeyActionCursorNavigationLeft"))
                CancelInvoke("KeyActionCursorNavigationLeft");

            if (IsInvoking("KeyActionCursorNavigationRight"))
                CancelInvoke("KeyActionCursorNavigationRight");
        }

        private void EnableAllButtons()
        {
            for (int i = 0; i < _keys.Length; i++)
                _keys[i].ChangeState(PYButtonState.Idle);
        }

        #endregion privates
    }
}