using UnityEngine;
using System.Collections;

namespace Playmove
{
    public class ButtonCheckbox : PYButton
    {

        public PYAudioTags MarkVoice, DesmarkVoice;
        private GameObject _checked;

        protected override void Start()
        {
            base.Start();
            _checked = transform.GetChild(0).gameObject;
        }

        protected override void ClickAction()
        {
            base.ClickAction();
            if (PYAudioManager.Instance != null)
                PYAudioManager.Instance.StartAudio(_checked.activeSelf ? DesmarkVoice : MarkVoice).Play();
            _checked.SetActive(!_checked.activeSelf);
        }
    }
}