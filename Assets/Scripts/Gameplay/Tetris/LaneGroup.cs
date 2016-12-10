using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;
using System.Linq;

namespace MDS.Gameplay.Tetris
{
    public class LaneGroup : MDSBehaviour
    {
        public List<Lane> lanes;
	
		protected override void Awake ()
		{
			base.Awake ();
			Initialize ();
		}

		private void Initialize()
		{
			int maxLanes = transform.childCount;
			for(int i = 0; i < maxLanes; i++)
			{
				lanes.Add(transform.GetChild(i).GetComponent<Lane>());
			}
		}
    }
}