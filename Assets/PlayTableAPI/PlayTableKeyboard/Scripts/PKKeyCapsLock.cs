using UnityEngine;
using System.Collections;

namespace Playmove
{
    public class PKKeyCapsLock : PYButton
    {
        [SerializeField]
        private Sprite Normal;
        [SerializeField]
        private Sprite Clicked;
        [SerializeField]
        private Color NormalIcon;
        [SerializeField]
        private Color ClickedIcon;

        private bool _active = true;

        private SpriteRenderer _mySpriteRenderer;
        private SpriteRenderer MySpriteRenderer
        {
            get
            {
                if (!_mySpriteRenderer) _mySpriteRenderer = GetComponent<SpriteRenderer>();
                return _mySpriteRenderer;
            }
        }
        private SpriteRenderer _icon;
        private SpriteRenderer Icon
        {
            get
            {
                if (!_icon) _icon = transform.GetChild(0).GetComponent<SpriteRenderer>();
                return _icon;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            SetSprite();
        }

        protected override void ClickAction()
        {
            base.ClickAction();
            _active = !_active;

            SetSprite();
        }

        private void SetSprite()
        {
            if (_active)
            {
                MySpriteRenderer.sprite = Clicked;
                Icon.color = ClickedIcon;
            }
            else
            {
                MySpriteRenderer.sprite = Normal;
                Icon.color = NormalIcon;
            }
        }
    }
}