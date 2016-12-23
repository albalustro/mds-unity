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
		[InspectorComment("Indicar o GO parent dos itens dropados quando utilizada a opção Change Parent, para lipar todos os itens dropados.")]
		public Transform dropedItensParent;

        public override IEnumerator Execute()
        {
            yield return base.Execute();
            if (dropArea!=null)
                dropArea.ResetGroup();

            if (initialGroup!=null)
                initialGroup.ResetInitialInstanceGroup();

			if (dropedItensParent != null)
			{
				while (dropedItensParent.childCount > 0) {
					Transform c = dropedItensParent.GetChild(0); 
					c.SetParent(null);
					MonoBehaviour.Destroy(c.gameObject);
				}
			}
        }
    }
}