using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Playmove
{
    public struct BitmapFontInfo
    {
        public float Kerning;
        public float LineSpacing;
        public int FontSize;
        public int CharacterSpacing;
        public int CharacterPadding;

        public BitmapFontInfo(float kerning, float lineSpacing, int fontSize, int characterSpacing, int characterPadding)
        {
            Kerning = kerning;
            LineSpacing = lineSpacing;
            FontSize = fontSize;
            CharacterSpacing = characterSpacing;
            CharacterPadding = characterPadding;
        }
    }

    public class GenerateBitmapFont
    {

        private static string _fontName;
        private static string _fntFullPath;
        private static string _fntPath;

        private static Font _font;
        private static Texture2D _fontTexture;
        private static Material _fontMaterial;

        private static int _charIndex;
        private static CharacterInfo[] _charInfo;

        private static BitmapFontInfo _fontInfo = new BitmapFontInfo(1, 0.1f, 1, 1, 0);

        [MenuItem("Assets/Generate Bitmap Font")]
        public static void GenerateFont()
        {
            Object selection = Selection.objects[0];
            if (!ValidateSelection(selection))
            {
                Debug.LogError("Please select a .fnt file");
                return;
            }

            TextAsset fnt = (TextAsset)selection;
            _fontName = fnt.name;
            _fntFullPath = AssetDatabase.GetAssetPath(selection);
            _fntPath = _fntFullPath.Replace(_fontName + ".fnt", "").Trim();

            string[] fntContent = fnt.text.Split("\n"[0]);

            _font = GetFont();

            _fontTexture = GetTexture();
            if (_fontTexture == null)
            {
                Debug.LogError("Font texture not found");
                return;
            }

            _charIndex = 0;
            for (int i = 0; i < fntContent.Length; i++)
                ReadLine(fntContent[i]);
            _font.characterInfo = _charInfo;

            _font.material = GetMaterial();

            SerializeProperties();

            Debug.Log("Font " + _fontName + " successfully generated!");
        }

        private static bool ValidateSelection(Object selection)
        {
            if (selection == null)
                return false;

            string[] splittedAssetPath = AssetDatabase.GetAssetPath(selection).Split('.');
            if (splittedAssetPath[splittedAssetPath.Length - 1] != "fnt")
                return false;

            return true;
        }

        private static Font GetFont()
        {
            string finalFontPath = _fntFullPath.Replace(".fnt", ".fontsettings");
            Font font;

            font = AssetDatabase.LoadAssetAtPath(finalFontPath, typeof(Font)) as Font;

            if (font == null)
            {
                font = new Font();
                AssetDatabase.CreateAsset(font, finalFontPath);
                font = AssetDatabase.LoadAssetAtPath(finalFontPath, typeof(Font)) as Font;
            }

            return font;
        }

        private static Texture2D GetTexture()
        {
            Texture2D texture;
            texture = AssetDatabase.LoadAssetAtPath(_fntFullPath.Replace(".fnt", ".png"), typeof(Texture2D)) as Texture2D;

            if (texture == null)
            {
                string textureFullPath = EditorUtility.OpenFilePanel("Select font texture file", _fntPath, "png");
                string texturePath = "Assets" + textureFullPath.Substring(Application.dataPath.Length);
                Debug.Log(texturePath);
                texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            }

            return texture;
        }

        private static Material GetMaterial()
        {
            string finalMaterialPath = _fntFullPath.Replace(".fnt", ".mat");

            if (_font.material != null)
            {
                _fontMaterial = _font.material;
                return _fontMaterial;
            }

            _fontMaterial = AssetDatabase.LoadAssetAtPath(finalMaterialPath, typeof(Material)) as Material;

            if (_fontMaterial == null)
            {
                _fontMaterial = new Material(Shader.Find("TextFX/Text Shader"));
                _fontMaterial.mainTexture = _fontTexture;

                AssetDatabase.CreateAsset(_fontMaterial, finalMaterialPath);
                _fontMaterial = AssetDatabase.LoadAssetAtPath(finalMaterialPath, typeof(Material)) as Material;
            }

            return _fontMaterial;
        }

        private static void SerializeProperties()
        {
            SerializedObject so = new SerializedObject(_font);
            so.FindProperty("m_Kerning").floatValue = _fontInfo.Kerning;
            so.FindProperty("m_LineSpacing").floatValue = _fontInfo.LineSpacing;
            so.FindProperty("m_FontSize").floatValue = _fontInfo.FontSize;
            so.FindProperty("m_CharacterSpacing").intValue = _fontInfo.CharacterSpacing;
            so.FindProperty("m_CharacterPadding").intValue = _fontInfo.CharacterPadding;

            so.ApplyModifiedProperties();
        }

        #region Readings
        private static void ReadLine(string line)
        {
            string[] lineContent = line.Split(' ');

            switch (lineContent[0])
            {
                case "info":
                    ReadInfo(lineContent);
                    break;

                case "common":
                    ReadCommon(lineContent);
                    break;

                case "page":
                    ReadPage(lineContent);
                    break;

                case "chars":
                    ReadCharsCount(lineContent);
                    break;

                case "char":
                    ReadChar(lineContent);
                    _charIndex++;
                    break;

                case "kernings":
                    ReadKernings(lineContent);
                    break;

                default:
                    break;
            }
        }

        private static void ReadInfo(string[] lineContent)
        {
            for (int i = 0; i < lineContent.Length; i++)
            {
                string[] info = lineContent[i].Split('=');

                switch (info[0])
                {
                    case "face":
                        break;

                    case "size":
                        _fontInfo.FontSize = int.Parse(info[1]);
                        break;

                    case "bold":
                        break;

                    case "italic":
                        break;

                    case "charset":
                        break;

                    case "unicode":
                        break;

                    case "stretchH":
                        break;

                    case "smooth":
                        break;

                    case "aa":
                        break;

                    case "padding":
                        _fontInfo.CharacterPadding = int.Parse(info[1].Split(',')[0]);
                        break;

                    case "spacing":
                        _fontInfo.CharacterSpacing = int.Parse(info[1].Split(',')[0]);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void ReadCommon(string[] lineContent)
        {
            for (int i = 0; i < lineContent.Length; i++)
            {
                string[] common = lineContent[i].Split('=');

                switch (common[0])
                {
                    case "lineHeight":
                        _fontInfo.LineSpacing = float.Parse(common[1]);
                        break;

                    case "base":
                        break;

                    case "scaleW":
                        break;

                    case "scaleH":
                        break;

                    case "pages":
                        break;

                    case "packed":
                        break;

                    case "alphaChnl":
                        break;

                    case "redChnl":
                        break;

                    case "greenChnl":
                        break;

                    case "blueChnl":
                        break;

                    default:
                        break;
                }
            }
        }

        private static void ReadPage(string[] lineContent)
        {

        }

        private static void ReadCharsCount(string[] lineContent)
        {
            int charCount = int.Parse(lineContent[1].Split('=')[1]);

            _charInfo = new CharacterInfo[charCount];
        }

        private static void ReadChar(string[] lineContent)
        {
            Rect uv = new Rect();
            Rect vert = new Rect();

            for (int i = 0; i < lineContent.Length; i++)
            {
                string[] data = lineContent[i].Split('=');

                switch (data[0])
                {
                    case "id":
                        _charInfo[_charIndex].index = int.Parse(data[1]);
                        break;

                    case "x":
                        float x = float.Parse(data[1]);
                        uv.x = x / _fontTexture.width;
                        break;

                    case "y":
                        float y = float.Parse(data[1]);
                        uv.y = y / _fontTexture.height;
                        break;

                    case "width":
                        float width = float.Parse(data[1]);
                        uv.width = width / _fontTexture.width;
                        vert.width = width;
                        break;

                    case "height":
                        float height = float.Parse(data[1]);
                        uv.height = height / _fontTexture.height;
                        uv.y = 1 - uv.height - uv.y;
                        vert.height = -height;
                        break;

                    case "xoffset":
                        break;

                    case "yoffset":
                        int yoffset = int.Parse(data[1]);
                        vert.y = -yoffset;
                        break;

                    case "xadvance":
                        _charInfo[_charIndex].width = int.Parse(data[1]); // Unity informou que fara a propriedade with de volta com advanced e será de escrita e leitura
                        break;

                    case "page":
                        break;

                    case "chnl":
                        break;

                    case "letter":
                        break;

                    default:
                        break;
                }

            }

            _charInfo[_charIndex].uv = uv;
            _charInfo[_charIndex].vert = vert;
        }

        private static void ReadKernings(string[] lineContent)
        {
            //int kerning = int.Parse(lineContent[1].Split('=')[1]);
            //_fontInfo.Kerning = kerning;
        }
        #endregion
    }
}