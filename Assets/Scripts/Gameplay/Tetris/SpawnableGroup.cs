using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDS.Gameplay.Tetris
{
	public class SpawnableGroup : MDSBehaviour {

		public List<Spawnable> spawnables;

		protected override void Awake ()
		{
			base.Awake ();
			Initialize ();
		}

		private void Initialize()
		{
			int maxSpawnables = transform.childCount;
			for(int i = 0; i < maxSpawnables; i++)
			{
				spawnables.Add(transform.GetChild(i).GetComponent<Spawnable>());
			}
		}
	}
}