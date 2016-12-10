using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;

namespace MDS.Gameplay.Tetris
{
	[RequireComponent(typeof(PolygonCollider2D))]
    public class Lane : MDSBehaviour
    {
        public Transform spawnPosition;
        public ValidatableLaneArea destinationPoint;
		private TetrisController _controller;

		public TetrisController Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}
    
		void OnMouseUp()
		{
			_controller.ChangeLane (this);
		}

		public bool Validate(Spawnable curSpawnable)
		{
			return destinationPoint.validLabel.Contains(curSpawnable.labels[0]);
		}
    }
}