using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDS.Player
{
    public class AvatarSelection : MDSBehaviour
    {
        [System.Serializable]
        public class Selection
        {
            public GameObject go;
            public Vector3 unselectedLocalPosition;
            public Vector3 selectedLocalPosition;
            public int orderInLayer;
            [FullInspector.NotSerialized, HideInInspector]
            public bool isSelected;

#if UNITY_EDITOR
            [FullInspector.InspectorButton]
            void CaptureSelectedPosition()
            {
                selectedLocalPosition = go.transform.localPosition;
            }

            [FullInspector.InspectorButton]
            void CaptureUNSelectedPosition()
            {
                unselectedLocalPosition = go.transform.localPosition;
            }
#endif

        }

        [FullInspector.ShowInInspector]
        public Dictionary<PlayerAvatar, Selection> avatarSelectionList;
        

        public void SelectAvatar(GameObject avatar)
        {
            foreach(var item in avatarSelectionList)
            {
                if(item.Value.go == avatar)
                    Select(item.Value, item.Key);
                else
                    Unselect(item.Value);
            }
        }

        private void Unselect(Selection value)
        {
            LeanTween.scale(value.go, Vector3.one, 0.3f);
            LeanTween.moveLocal(value.go, value.unselectedLocalPosition, 0.3f);
            value.go.GetComponent<Renderer>().sortingOrder = value.orderInLayer;
            value.isSelected = false;
        }

        private void Select(Selection value, PlayerAvatar avatar)
        {
            LeanTween.scale(value.go, 1.2f * Vector3.one, 0.3f);
            LeanTween.moveLocal(value.go, value.selectedLocalPosition, 0.3f);
            value.go.GetComponent<Renderer>().sortingOrder = 15;
            value.isSelected = true;
            PlayerAnimController.OriginalAvatar = avatar;
        }
    }
}