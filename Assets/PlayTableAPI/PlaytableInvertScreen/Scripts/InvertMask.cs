using UnityEngine;
using System.Collections;
using System;

namespace Playmove
{
    public class InvertMask : MonoBehaviour
    {

        float scaleSpeed = 10f;

        Material mat;

        void Start()
        {
            mat = GetComponent<Renderer>().material;
        }

        Action callback;
        public void Animate(Action callback)
        {
            this.callback = callback;
            StartCoroutine(AnimateTransition());
        }

        float duration = 0.5f;
        IEnumerator AnimateTransition(bool isIn = true)
        {
            float rotation = 0;
            float time = 0;

            GetComponent<Renderer>().enabled = true;

            mat = GetComponent<Renderer>().material;

            if (isIn)
            {
                float scale = -0.4f;
                while (time < duration)
                {
                    time += Time.deltaTime;
                    rotation = Mathf.Lerp(0, 6.3f, time / duration);
                    mat.SetFloat("_Rotation", rotation);
                    scale += scale * scaleSpeed * Time.deltaTime;
                    mat.SetFloat("_Scale", scale);
                    yield return null;
                }
            }
            else
            {
                float scale = -60;
                while (time < duration)
                {
                    time += Time.deltaTime;
                    rotation = Mathf.Lerp(0, 6.3f, time / duration);
                    mat.SetFloat("_Rotation", rotation);
                    scale -= scale * scaleSpeed * Time.deltaTime;
                    mat.SetFloat("_Scale", scale);
                    yield return null;
                }

                GetComponent<Renderer>().enabled = false;
            }

            float s = (isIn ? -1000 : 0);
            mat.SetFloat("_Scale", s);

            if (isIn)
            {
                if (callback != null)
                    callback();

                yield return new WaitForSeconds(0.2f);
                StartCoroutine(AnimateTransition(!isIn));
            }
        }
    }
}