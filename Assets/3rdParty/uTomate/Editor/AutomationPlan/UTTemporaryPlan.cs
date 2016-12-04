//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEngine;

    /// <summary>
    /// A temporary plan used to run single actions.
    /// </summary>
    public class UTTemporaryPlan
    {
        private UTAutomationPlan plan;
        private UTAutomationPlanSingleActionEntry entry;

        public UTTemporaryPlan(UTAction action)
        {
            plan = ScriptableObject.CreateInstance<UTAutomationPlan>();
            entry = ScriptableObject.CreateInstance<UTAutomationPlanSingleActionEntry>();
            entry.action = action;
            plan.firstEntry = entry;
        }

        /// <summary>
        /// Runs the temporary plan.
        /// </summary>
        public void Run()
        {
            UTomateRunner.Instance.OnRunnerFinished += CleanUp;
            UTomate.Run(plan);
        }

        public void CleanUp(bool canceled, bool failed)
        {
            UTomateRunner.Instance.OnRunnerFinished -= CleanUp;
            ScriptableObject.DestroyImmediate(plan);
            ScriptableObject.DestroyImmediate(entry);
        }
    }
}
