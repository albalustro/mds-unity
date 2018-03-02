using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Playmove
{
    public class EditorCoroutine
    {
        public static EditorCoroutine Start(IEnumerator routine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(routine);
            coroutine.Start();
            return coroutine;
        }

        public readonly IEnumerator Routine;

        private EditorCoroutine(IEnumerator routine)
        {
            Routine = routine;
        }

        public void Stop()
        {
            EditorApplication.update -= UpdateRoutine;
        }

        void Start()
        {
            EditorApplication.update += UpdateRoutine;
        }

        void UpdateRoutine()
        {
            if (!Routine.MoveNext())
                Stop();
        }
    }
}