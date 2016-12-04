//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
	using API;
	using System.Collections;
	using UnityEditor;

	[UTActionInfo(actionCategory = "Bake", sinceUTomateVersion="1.5.0")]
	[UTDoc(title = "Clear Lightmaps", description = "Removes previously baked lightmaps for the currently open scene.")]
	[UTDefaultAction]
	public class UTClearLightmapsAction : UTAction {
	
		public override IEnumerator Execute(UTContext context)
		{
			Lightmapping.Clear();
			yield return "";
		}

		[MenuItem("Assets/Create/uTomate/Bake/Clear Lightmaps", false, 215)]
		public static void AddAction()
		{
			Create<UTClearLightmapsAction>();
		}

	}

}
