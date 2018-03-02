using UnityEngine;
using System.Collections;

namespace Playmove
{
    public class PKKeyboardPageAnimator : MonoBehaviour
    {

        public GameObject NormalPage;
        public GameObject SpecialPage;

        private PYTweenAnimation tween;

        public void TurnNormalPage(bool reverse, System.Action callback = null)
        {
            TurnPage(NormalPage, reverse, callback);
        }

        public void TurnSpecialPage(bool reverse, System.Action callback = null)
        {
            TurnPage(SpecialPage, reverse, callback);
        }

        private void TurnPage(GameObject page, bool reverse, System.Action callback = null)
        {
            tween = PYTweenAnimation.Add(page);
            //tween.Rotation (startRotation, targetRotation);
            tween.SetScale(Vector3.one, Vector3.up);
            tween.SetDuration(0.2f);
            if (!reverse)
                tween.Play(callback);
            else
                tween.Reverse(callback);
        }
    }
}