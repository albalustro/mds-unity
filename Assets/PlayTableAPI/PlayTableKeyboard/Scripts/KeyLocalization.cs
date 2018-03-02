using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Playmove
{
    public class KeyLocalization : MonoBehaviour
    {

        [System.Serializable]
        public struct Key
        {
            public string Language;
            public string Letter;
        }

        [SerializeField]
        private string Default = "Ç";
        [SerializeField]
        private List<Key> KeyLocalizations = new List<Key>();

        private PKKeyButton _myBtn;
        private PKKeyButton MyBtn
        {
            get
            {
                if (!_myBtn) _myBtn = GetComponent<PKKeyButton>();
                return _myBtn;
            }
        }

        private void Awake()
        {
            MyBtn.Text = Default;
            foreach (Key key in KeyLocalizations)
            {
                if (key.Language.ToLower() == PYBundleManager.Instance.Language.ToLower())
                    MyBtn.Text = key.Letter;
            }
        }
    }
}