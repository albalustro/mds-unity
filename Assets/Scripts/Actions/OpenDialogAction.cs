using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Actions
{
    [Serializable]
    public class OpenDialogAction : IAction
    {
        public Slug[] slugs { get; set; }
        public IEnumerator Execute(Action callback)
        {
            DialogueSystem.instance.ShowDialogueMessage(slugs);
            yield return new WaitWhile(DialogueSystem.instance.IsDialogueOpen);
        }
    }
}
