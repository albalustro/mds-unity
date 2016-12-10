using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;

namespace MDS.Gameplay.Tetris
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Spawnable : MDSBehaviour
    {
        public List<string> labels;
    }
}