//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate.API
{
    using System.Collections;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Base class for all runnable actions.
    /// </summary>
    [UTDoc(title = "General")]
    public abstract class UTAction : ScriptableObject
    {
        /// <summary>
        /// Execute this instance.
        /// </summary>
        /// <param name="context">The current build context.</param>
        public abstract IEnumerator Execute(UTContext context);

        /// <summary>
        /// The action version. This is used to facilitate later migration steps if actions change.
        /// </summary>
        public static string ActionVersion = "1.7";    //
                                                       // 1.2 for UTBuildPlayerAction, UTSetPlayerSettingsIosAction
                                                       // 1.3 for UTRunUTomateInExternalProjectAction
                                                       // 1.4 for UTUploadToTestFairyAction
                                                       // 1.5 for UTBuildXcodeProjectAction
                                                       // 1.6 for UTSetPlayerSettingsIosAction
                                                       // 1.7 for UTSetPlayerSettingsPcMacAction
        [HideInInspector]
        public string CreatedWithActionVersion;

        /// <summary>
        /// Creates a new UBuildAction of the given type.
        /// </summary>
        /// <typeparam name='T'>
        /// The type of action to create.
        /// </typeparam>
        public static T Create<T>() where T : UTAction
        {
            UTDoc doc = UTDoc.GetFor(typeof(T));
            var result = UTils.CreateAssetOfType<T>(doc.title);
            result.CreatedWithActionVersion = ActionVersion;
            return result;
        }

        /// <summary>
        /// Evaluates the given array of UTProperties in the given context and returns an array of the
        /// evaluated values.
        /// </summary>
        public static T[] EvaluateAll<T>(UTProperty<T>[] propertyArray, UTContext context)
        {
            if (propertyArray == null)
            {
                return new T[0];
            }
            var result = new T[propertyArray.Length];
            for (int i = 0; i < propertyArray.Length; i++)
            {
                result[i] = propertyArray[i].EvaluateIn(context);
            }
            return result;
        }

        [ContextMenu("Run this action")]
        public void RunThisAction()
        {
            if (UTomateRunner.Instance.IsRunning)
            {
                Debug.LogWarning("There is currently a plan running.");
                return;
            }
            new UTTemporaryPlan(this).Run();
        }

        [MenuItem("Assets/Run this action", true)]
        private static bool ValidateRunSelectedAction()
        {
            return Selection.activeObject is UTAction;
        }

        [MenuItem("Assets/Run this action", false, 690)]
        private static void RunSelectedAction()
        {
            (Selection.activeObject as UTAction).RunThisAction();
        }

        public override string ToString()
        {
            return name;
        }
    }
}
