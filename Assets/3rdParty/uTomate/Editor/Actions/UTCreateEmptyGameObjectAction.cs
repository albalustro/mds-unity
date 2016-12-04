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
    using UnityEngine;

    [UTActionInfo(actionCategory = "Scene Manipulation", sinceUTomateVersion = "1.7.0")]
    [UTDoc(title = "Create Empty Game Object", description = "Creates an empty game object.")]
    [UTDefaultAction]
    public class UTCreateEmptyGameObjectAction : UTAction
    {       

        [UTDoc(description = "The name of the new object. Leave empty to use the default name.")]
        [UTInspectorHint(order = 1)]
        public UTString objectName;

        [UTDoc(description = "Name of the property into which the new object should be stored. Leave empty if you don't need the object for further actions.")]
        public UTString outputProperty;

        public override IEnumerator Execute(UTContext context)
        {
            var theObjectName = objectName.EvaluateIn(context);
            var theProperty = outputProperty.EvaluateIn(context);
            
            var theObject = new GameObject();

            if (!string.IsNullOrEmpty(theObjectName))
            {
                theObject.name = theObjectName;
            }

            if (!string.IsNullOrEmpty(theProperty))
            {
                context[theProperty] = theObject;
            }

            yield return "";
        }

        [MenuItem("Assets/Create/uTomate/Scene Manipulation/Create Empty Game Object", false, 501)]
        public static void AddAction()
        {
            Create<UTCreateEmptyGameObjectAction>();
        }
    }
}
