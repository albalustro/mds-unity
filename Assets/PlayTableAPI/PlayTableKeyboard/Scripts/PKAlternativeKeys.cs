using UnityEngine;
using System.Collections.Generic;

namespace Playmove
{
    public class PKAlternativeKeys : MonoBehaviour
    {
        private static List<GameObject> keys;

        public float HoldingTimeToShow = 1.5f;
        public string[] AlternativeChars;
        public GameObject AlternativeCharKey;
        public BoxCollider2D Box;

        private Vector3 keySize;
        private PYTweenAnimation tween;
        private Bounds boxBounds;

        public bool IsShowing { get; set; }

        void Start()
        {
            Box.gameObject.SetActive(true);

            if (keys == null)
            {
                keys = new List<GameObject>();
                for (int x = 0; x < 3; x++)
                {
                    GameObject key = (GameObject)Instantiate(AlternativeCharKey);
                    key.SetActive(false);
                    keys.Add(key);
                }
            }

            keySize = new Vector3(
                Box.bounds.size.x / AlternativeChars.Length,
                Box.bounds.size.y, 1);

            boxBounds = Box.bounds;

            Box.gameObject.SetActive(false);

            tween = Box.GetComponent<PYTweenAnimation>();
        }

        public void Show()
        {
            IsShowing = true;

            Box.gameObject.SetActive(true);
            if (AlternativeChars.Length > keys.Count)
            {
                int keyCount = keys.Count;
                for (int x = 0; x < AlternativeChars.Length - keyCount; x++)
                {
                    GameObject key = (GameObject)Instantiate(AlternativeCharKey);
                    key.SetActive(false);
                    keys.Add(key);
                }
            }

            for (int i = 0; i < AlternativeChars.Length; i++)
            {
                Box.transform.localScale = Vector3.one;
                keys[i].transform.position = new Vector3(boxBounds.min.x + (keySize.x * i) + keySize.x / 2, Box.transform.position.y, Box.transform.position.z - 5);
                keys[i].transform.parent = Box.transform;
                keys[i].transform.localScale = Vector3.one;

                PKKeyButton keyButton = keys[i].GetComponent<PKKeyButton>();
                //keyButton.Graphic.transform.localScale = keySize;
                keyButton.Text = AlternativeChars[i];

                keys[i].SetActive(true);
            }

            Box.enabled = false;

            tween.SetDuration(0.75f);
            tween.SetEaseType(Ease.Type.OutElastic);
            tween.Play();
        }

        public void Hide()
        {
            IsShowing = false;

            tween.SetDuration(0.15f);
            tween.SetEaseType(Ease.Type.Linear);
            tween.Reverse(() =>
            {
                Box.enabled = true;
                Box.gameObject.SetActive(false);

                for (int i = 0; i < AlternativeChars.Length; i++)
                {
                    keys[i].transform.parent = null;
                    keys[i].SetActive(false);
                }
            });
        }

        public bool IsKey(GameObject key)
        {
            return keys.Contains(key);
        }
    }
}