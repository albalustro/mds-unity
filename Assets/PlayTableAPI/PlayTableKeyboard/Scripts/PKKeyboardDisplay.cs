using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Playmove
{
    public class PKKeyboardDisplay : MonoBehaviour
    {
        public float CharacterSpacing;
        public Font TextFont;
        public int FontSize;
        public Color TextColor;
        public Transform DisplayChars;
        public Sprite SlotGraphic;
        public Transform Cursor;
        public Vector3 CursorOffset = new Vector3(0, 0f, 0);
        public AudioClip DisplayClickSound;

        [SerializeField]
        private int _maxChars = 20;
        public int MaxChars { get { return _maxChars; } }

        private ParticleSystem cursorParticle;
        private TextMesh[] DisplayTextMeshes;

        void Start()
        {
            if (SlotGraphic != null)
                CharacterSpacing += SlotGraphic.bounds.size.x;

            cursorParticle = Cursor.GetComponentInChildren<ParticleSystem>();

            DisplayTextMeshes = new TextMesh[MaxChars];

            DisplayTextMeshes[0] = new GameObject("Display" + 0).AddComponent<TextMesh>();
            DisplayTextMeshes[0].font = TextFont;
            DisplayTextMeshes[0].fontSize = FontSize;
            DisplayTextMeshes[0].color = TextColor;
            DisplayTextMeshes[0].characterSize = 0.1f;
            DisplayTextMeshes[0].GetComponent<Renderer>().material = TextFont.material;
            DisplayTextMeshes[0].alignment = TextAlignment.Center;
            DisplayTextMeshes[0].anchor = TextAnchor.MiddleCenter;
            DisplayTextMeshes[0].transform.parent = DisplayChars;
            DisplayTextMeshes[0].transform.localPosition = Vector3.zero;
            DisplayTextMeshes[0].transform.localScale = Vector3.one;
            DisplayTextMeshes[0].gameObject.layer = gameObject.layer;
            SortingOrder so = DisplayTextMeshes[0].gameObject.AddComponent<SortingOrder>();

            SortingOrder sso = GetComponentInParent<SortingOrder>();
            so.AlwaysUpdate = true;
            so.Order = sso.Order + 1;
            so.SortLayerName = sso.SortLayerName;

            BoxCollider2D col = DisplayTextMeshes[0].gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(CharacterSpacing, 1);

            DisplayTextMeshes[0].gameObject.AddComponent<PYButton>().onClick.AddListener(OnCharacterDisplayClicked);

            for (int i = 1; i < DisplayTextMeshes.Length; i++)
            {
                DisplayTextMeshes[i] = ((GameObject)Instantiate(DisplayTextMeshes[0].gameObject)).GetComponent<TextMesh>();
                DisplayTextMeshes[i].name = "Display" + i;
                DisplayTextMeshes[i].transform.parent = DisplayChars;
                DisplayTextMeshes[i].transform.localPosition = Vector3.right * i * CharacterSpacing;
                DisplayTextMeshes[i].transform.localScale = Vector3.one;
                DisplayTextMeshes[i].gameObject.layer = gameObject.layer;

                DisplayTextMeshes[i].GetComponent<PYButton>().onClick.AddListener(OnCharacterDisplayClicked);
            }

            UpdateCursorPosition(1);

            PlayTableKeyboard.Instance.SetTextDelegates = SetText;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.K) && Input.GetKeyDown(KeyCode.R))
            {
                RepositionChars();
            }
        }

        private void RepositionChars()
        {
            for (int i = 0; i < DisplayTextMeshes.Length; i++)
                DisplayTextMeshes[i].transform.localPosition = Vector3.right * i * CharacterSpacing;
        }

        public Vector3 GetCharPosition(int index)
        {
            return DisplayTextMeshes[index].transform.position;
        }

        public void UpdateCursorPosition(int charIndex)
        {
            Cursor.position = GetCharPosition(charIndex - 1) + CursorOffset;
        }

        List<TextMesh> usedChars = new List<TextMesh>();
        GameObject centeredTextMeshParent = null;
        Vector3 previousPosition = Vector3.zero;
        public void CenterText()
        {
            if (string.IsNullOrEmpty(PlayTableKeyboard.Instance.Text)) return;

            usedChars = new List<TextMesh>();

            foreach (TextMesh tm in DisplayTextMeshes)
            {
                if (tm.text != "")
                {
                    usedChars.Add(tm);
                }
            }

            if (!centeredTextMeshParent)
            {
                centeredTextMeshParent = new GameObject("CenteredText");
            }
            centeredTextMeshParent.transform.SetParent(transform);
            centeredTextMeshParent.transform.localPosition = DisplayChars.localPosition;

            float posX = 0;
            for (int i = 0; i < usedChars.Count; i++)
            {
                posX += usedChars[i].transform.localPosition.x;
            }

            posX /= usedChars.Count;
            posX += DisplayChars.localPosition.x;

            centeredTextMeshParent.transform.localPosition = new Vector3(posX, 0, 0);

            for (int i = 0; i < usedChars.Count; i++)
            {
                usedChars[i].transform.SetParent(centeredTextMeshParent.transform);
            }

            previousPosition = centeredTextMeshParent.transform.localPosition;
            PYTweenAnimation.Add(centeredTextMeshParent).SetPosition(previousPosition, Vector3.zero, true).SetDuration(0.7f).SetEaseType(Ease.Type.InOutExpo).Play();

            EnableCursor(false);
        }

        public void ReturnText()
        {
            if (!centeredTextMeshParent || centeredTextMeshParent.transform.childCount == 0)
            {
                ResetDisplayPosition();
                return;
            }

            PYTweenAnimation.Add(centeredTextMeshParent).SetPosition(centeredTextMeshParent.transform.localPosition, previousPosition, true).SetDuration(0.7f).SetEaseType(Ease.Type.InOutExpo).Play();
            ResetDisplayPosition();
        }

        public void ResetDisplayPosition()
        {
            if (DisplayTextMeshes == null) return;
            for (int i = 0; i < DisplayTextMeshes.Length; i++)
            {
                DisplayTextMeshes[i].transform.SetParent(DisplayChars);
                DisplayTextMeshes[i].transform.localPosition = Vector3.right * i * CharacterSpacing;
                DisplayTextMeshes[i].transform.localScale = Vector3.one;
            }

            EnableCursor(true);
            PlayTableKeyboard.Instance.SetCursorPosition(0);
            UpdateCursorPosition(1);
        }

        public void EnableCursor(bool enabled)
        {
            Cursor.gameObject.SetActive(enabled);
        }

        void SetText(string text)
        {
            if (cursorParticle)
            {
                cursorParticle.Emit(30);
                cursorParticle.Play();
            }

            for (int i = 0; i < DisplayTextMeshes.Length; i++)
            {
                if (i < text.Length)
                    DisplayTextMeshes[i].text = text[i].ToString();
                else
                    DisplayTextMeshes[i].text = "";
            }
        }

        void OnCharacterDisplayClicked(PYButton sender)
        {
            if (sender.GetComponent<TextMesh>() == null) return;

            for (int i = 0; i < DisplayTextMeshes.Length; i++)
            {
                if (sender.gameObject == DisplayTextMeshes[i].gameObject)
                {
                    if (i < PlayTableKeyboard.Instance.Text.Length + 1)
                    {
                        UpdateCursorPosition(i + 1);
                        PlayTableKeyboard.Instance.SetCursorPosition(i);
                    }
                    else
                    {
                        UpdateCursorPosition(PlayTableKeyboard.Instance.Text.Length + 1);
                        PlayTableKeyboard.Instance.SetCursorPosition(PlayTableKeyboard.Instance.Text.Length);
                    }

                    PlayTableKeyboard.Instance.VerifyRulesAndEnableButtons();

                    if (DisplayClickSound != null)
                        PlayTableKeyboard.Instance.GetComponent<AudioSource>().PlayOneShot(DisplayClickSound);

                    break;
                }
            }
        }
    }
}