using UnityEngine;
using System.Collections;

namespace Playmove
{
    public class PYButtonAnimation : PYButton
    {
        public PYAnimation PressedAnimation;
        public PYAnimation ReleasedAnimation;
        public PYAnimation DisabledAnimation;

        protected override void Start()
        {
            base.Start();
        }

        protected override void DownAction()
        {
            if (PressedAnimation != null)
            {
                PressedAnimation.Stop();
                PressedAnimation.Play();
            }
        }

        protected override void UpAction()
        {
            if (ReleasedAnimation != null)
            {
                ReleasedAnimation.Stop();
                ReleasedAnimation.Play();
            }
            else if (PressedAnimation != null)
            {
                PressedAnimation.Stop();
                PressedAnimation.Reverse();
            }
        }

        protected override void DisableAction()
        {
            base.DisableAction();

            //Parando as demais animações
            if (PressedAnimation != null)
                PressedAnimation.Stop();

            if (ReleasedAnimation != null)
                ReleasedAnimation.Stop();

            if (DisabledAnimation != null)
                DisabledAnimation.Play();
        }

        protected override void EnableAction()
        {
            base.EnableAction();

            if (DisabledAnimation != null)
                DisabledAnimation.Reverse();
        }
    }
}