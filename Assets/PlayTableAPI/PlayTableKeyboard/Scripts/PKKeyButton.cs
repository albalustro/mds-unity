using UnityEngine;

namespace Playmove
{
    public enum PKKeyType
    {
        Letter,
        Space,
        Backspace,
        Confirm,
        Cancel,
        Special, //Accent buttons
        CursorNavigationLeft,
        CursorNavigationRight,
        ClearAllText,
        Mute,
        Point,
        CapsLock
    }

    [ExecuteInEditMode]
    public class PKKeyButton : PYButton
    {
        public GameObject Letter;
        public PKKeyType Type;

        [SerializeField]
        private SpriteRenderer _icon;

        [SerializeField]
        private Color _iconNormal;

        [SerializeField]
        private Color _iconDisable;

        [SerializeField, HideInInspector]
        private string _text;

        public string Text
        {
            get
            {
                return _text ?? string.Empty;
            }
            set
            {
                _text = value;
                if (TextComponent != null)
                    TextComponent.Text = value;
            }
        }

        [SerializeField]
        private PKKeyboardSkin customSkin;

        public PKKeyboardSkin CustomSkin
        {
            get
            {
                return customSkin;
            }
            set
            {
                customSkin = value;
                ChangeState(State);
                GetResourcesAudio(0);
            }
        }

        private SpriteRenderer _graphic;

        public SpriteRenderer Graphic
        {
            get
            {
                if (!_graphic) _graphic = transform.Find("KeyGraphic").GetComponent<SpriteRenderer>();
                return _graphic;
            }
            set
            {
                _graphic = value;
            }
        }

        private PYText _textComponent;

        public PYText TextComponent
        {
            get
            {
                if (!_textComponent)
                {
                    _textComponent = GetComponentInChildren<PYTextBox>();
                    if (!_textComponent)
                        _textComponent = GetComponentInChildren<PYText>();
                }
                return _textComponent;
            }
        }

        private string _language;

        private string Language
        {
            get
            {
                if (!string.IsNullOrEmpty(_language)) return _language;

                _language = PYBundleManager.Instance == null ? "pt-BR" : PYBundleManager.Instance.Language;
                return _language;
            }
        }

        [HideInInspector]
        public AudioClip KeyVoice;

        protected override void OnEnable()
        {
            base.Awake();
            CustomSkin = customSkin ?? PlayTableKeyboard.Instance.Skin;
        }

        public void SetKey(string letter, int page)
        {
            Text = letter;
            GetResourcesAudio(page);
        }

        public void ChangeState(PYButtonState newState)
        {
            _state = newState;
            switch (State)
            {
                case PYButtonState.Idle:
                    ApplyNormalStateSkin();
                    break;

                case PYButtonState.Pressed:
                    ApplyPressedStateSkin();
                    break;

                case PYButtonState.Disabled:
                    ApplyDisabledStateSkin();
                    break;
            }
        }

        public void ApplyNormalStateSkin()
        {
            Graphic.sprite = customSkin.Button.Normal;
            Graphic.color = customSkin.ButtonColor.Normal;

            if (TextComponent != null)
                TextComponent.Color = customSkin.TextColor.Normal;

            if (_icon != null)
                _icon.color = _iconNormal;
        }

        public void ApplyPressedStateSkin()
        {
            Graphic.sprite = customSkin.Button.Pressed == null ? customSkin.Button.Normal : customSkin.Button.Pressed;
            Graphic.color = customSkin.ButtonColor.Pressed;

            if (TextComponent != null)
                TextComponent.Color = customSkin.TextColor.Pressed;

            if (_icon != null)
                _icon.color = _iconNormal;
        }

        public void ApplyDisabledStateSkin()
        {
            Graphic.sprite = customSkin.Button.Disabled == null ? customSkin.Button.Normal : customSkin.Button.Disabled;
            Graphic.color = customSkin.ButtonColor.Disabled;

            if (TextComponent != null)
                TextComponent.Color = customSkin.TextColor.Disabled;

            if (_icon != null)
                _icon.color = _iconDisable;
        }

        private void GetResourcesGraphics()
        {
#if !UNITY_EDITOR
        customSkin = (PKKeyboardSkin)Instantiate(customSkin);
#endif
            customSkin.Button.Normal = Resources.Load<Sprite>(customSkin.KeyTag + "/" + Language + "/" + Text.ToLower());
            customSkin.Button.Pressed = Resources.Load<Sprite>(customSkin.KeyTag + "/" + Language + "/" + Text.ToLower() + "_HIT");
            customSkin.Button.Disabled = Resources.Load<Sprite>(customSkin.KeyTag + "/" + Language + "/" + Text.ToLower() + "_DISABLE");
        }

        private void GetResourcesAudio(int page)
        {
            string resourcePath = string.Format("Localization/{0}/Narration/", PlaytableWin32.Instance.Language);
            if (customSkin.KeyTag != null && page < customSkin.KeyTag.Length && !string.IsNullOrEmpty(customSkin.KeyTag[page]))
            {
                resourcePath += customSkin.KeyTag[page];
            }
            else
                resourcePath += Text.ToLower();

            KeyVoice = Resources.Load<AudioClip>(resourcePath);
        }
    }
}