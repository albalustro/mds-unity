using System.Collections;
using UnityEngine;
using MDS.Core.Interfaces;
using System;
using MDS.Gameplay.DragDrop;
using FullInspector;

namespace MDS.Actions
{
    public class ResetInitial : BaseAction
    {
		[InspectorComment("Usado em casos onde necessita-se limpar o infinity bag")]
        public ValidatableDropGroupArea dropArea;
		[InspectorComment("Usado em desafios onde o item do initial instance muda a cada drop (possui um array de itens). Ele limpa todos e volta para o primeiro")]
        public InitialIntanceDropGroupArea initialGroup;

        public override IEnumerator Execute()
        {
            yield return base.Execute();
            if (dropArea!=null)
                dropArea.ResetGroup();

            if (initialGroup!=null)
                initialGroup.ResetInitialInstanceGroup();
        }
    }
}