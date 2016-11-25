using UnityEngine;

using UnityEditor;

public class AlignChildren : Editor {

	[MenuItem("MDS/Align Children/Vertical")]
	public static void VerticalSpacer()
	{

		Transform transform = Selection.activeTransform;

		Vector3 first = transform.GetChild(0).localPosition;
		Vector3 last = transform.GetChild(transform.childCount - 1).localPosition;

		int max = transform.childCount -1;
		float step = (last.x - first.x)/max;

		for(int i = 1; i < max; i++)
		{
			Vector3 cur = transform.GetChild(i).localPosition;
			cur.x = first.x + i * step;
			transform.GetChild(i).localPosition = cur;
		}


	}
	
}
