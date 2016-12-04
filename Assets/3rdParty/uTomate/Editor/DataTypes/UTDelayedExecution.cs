//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using API;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class UTDelayedExecution : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private UTAutomationPlan runAfterDeserialization;

        public UTAutomationPlan RunAfterDeserialization
        {
            set { runAfterDeserialization = value; }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            EditorApplication.update += Revive;
        }

        private void Revive()
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Running automation plan " + runAfterDeserialization.name);
            }
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= Revive;
            UTomate.Run(runAfterDeserialization);
        }
    }
}
