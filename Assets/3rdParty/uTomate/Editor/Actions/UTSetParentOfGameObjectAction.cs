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

    [UTActionInfo(actionCategory = "Scene Manipulation", sinceUTomateVersion = "1.7.0")]
    [UTDoc(title = "Set Parent of Game Object", description = "Sets the parent of a given game object.")]
    [UTDefaultAction]
    public class UTSetParentOfGameObjectAction : UTAction
    {
        [UTDoc(description = "The game object for which the parent should be set.")]
        [UTInspectorHint(required = true, order = 0)]
        public UTGameObject gameObject;

        [UTDoc(description = "The new parent of the game object. If this is empty, the game object will become a top level game object.")]
        [UTInspectorHint(required = false, order = 1)]
        public UTGameObject parent;

        [UTDoc(description = "	If true, the parent-relative position, scale and rotation is modified such that the object keeps the same world space position, rotation and scale as before.")]
        public UTBool keepWorldPosition;



        public override IEnumerator Execute(UTContext context)
        {
            var theGameObject = gameObject.EvaluateIn(context);
            if (theGameObject == null)
            {
                throw new UTFailBuildException("You need to specify a game object for which the parent should be set.", this);
            }
            var doKeepWorldPosition = keepWorldPosition.EvaluateIn(context);
            var theParent = parent.EvaluateIn(context);

            theGameObject.transform.SetParent(theParent == null ? null : theParent.transform, doKeepWorldPosition);

            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Scene Manipulation/Set Parent of Game Object", false, 519)]
        public static void AddAction()
        {
            var theAction = Create<UTSetParentOfGameObjectAction>();

            var keepWorldPosition = new UTBool();
            keepWorldPosition.StaticValue = true;
            theAction.keepWorldPosition = keepWorldPosition;
        }
    }
}
