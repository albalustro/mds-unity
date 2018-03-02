using UnityEngine;
using System.Collections;

namespace Playmove
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PYButtonToggleColor : PYButtonToggle
    {

        private SpriteRenderer _mySpriteRenderer;
        private SpriteRenderer MySpriteRenderer
        {
            get
            {
                if (_mySpriteRenderer == null)
                    _mySpriteRenderer = GetComponent<SpriteRenderer>();
                return _mySpriteRenderer;
            }
        }

        public Color DeselectColor = Color.white, SelectColor = Color.gray;

        public override void Select()
        {
            base.Select();

            MySpriteRenderer.color = SelectColor;
        }

        public override void Deselect()
        {
            base.Deselect();

            MySpriteRenderer.color = DeselectColor;
        }

    }
}