using UnityEngine;
using System.Collections;

namespace Playmove
{
    [System.Serializable]
    public class PKKeyboardSkin : ScriptableObject
    {
        [System.Serializable]
        public class ColorByState
        {
            public Color Normal = Color.black;
            public Color Pressed = Color.gray;
            public Color Disabled = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        [System.Serializable]
        public class FontSizeByState
        {
            public int Normal = 32;
            public int Pressed = 24;
            public int Disabled = 24;
        }

        [System.Serializable]
        public class SpriteByState
        {
            public Sprite Normal;
            public Sprite Pressed;
            public Sprite Disabled;
        }

        public Font TextFont;
        public bool ShowText = true;
        public ColorByState TextColor;
        public FontSizeByState TextSize;
        public SpriteByState Button;
        public ColorByState ButtonColor;
        public AudioClip KeySound;
        public string[] KeyTag;
    }
}