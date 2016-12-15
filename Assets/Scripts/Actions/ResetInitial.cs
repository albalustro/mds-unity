using System.Collections;
using UnityEngine;
using MDS.Core.Interfaces;
using System;
using MDS.Gameplay.DragDrop;

namespace MDS.Actions
{
    public class ResetInitial : BaseAction
    {
        public ValidatableDropGroupArea dropArea;
        public InitialIntanceDropGroupArea initialGroup;

        public override IEnumerator Execute()
        {
            yield return base.Execute();
            dropArea.ResetGroup();
            initialGroup.ResetInitialInstanceGroup();
        }
    }
}