using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;

namespace MDS.Gameplay.Tetris
{
    public class SpawnableGroupSlot : MDSBehaviour
    {





#region Unity Editor Only
#if UNITY_EDITOR
    Color editorBoundColor = Color.yellow;
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