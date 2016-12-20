using System.Collections;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MDS.Gameplay.DragDrop
{
    public class MatrixValidatableDropGroup : ValidatableDropGroupArea
    {
        
        private int rows = 3;
        private int cols = 4;

        private int shapeLabelIndex = 0;
        private int colorLabelIndex = 1;

        public override bool Validate(string acceptableAnswer)
        {

            // validando colunas (color)
            for(int i = 0; i < cols; i++)
            {
                string validColLabel = slots[i].draggableReference.Labels[colorLabelIndex];
                for(int j = 0; j < rows; j++)
                {
                    if(slots[i + j * cols].Validate(validColLabel) == false)
                        return false;
                }
            }

            // validando linhas (shape)
            for(int j = 0; j < rows; j++)
            {
                string validRowLabel = slots[j * cols].draggableReference.Labels[shapeLabelIndex];
                for(int i = 0; i < cols; i++)
                {
                    if(slots[i + j * cols].Validate(validRowLabel) == false)
                        return false;
                }
            }


            

            return true;

        }
    }
}