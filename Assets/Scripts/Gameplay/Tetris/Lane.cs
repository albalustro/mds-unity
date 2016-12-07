using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;

namespace MDS.Gameplay.Tetris
{
    public class Lane : MDSBehaviour
    {
        public Transform spawnPosition;
        public ValidatableLaneArea destinationPoint;
    


#region Unity Editor Only
#if UNITY_EDITOR

    Color editorBoundColor = Color.cyan;
    void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.color = editorBoundColor;
            Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
        }
    }
#endif
#endregion

    }
}