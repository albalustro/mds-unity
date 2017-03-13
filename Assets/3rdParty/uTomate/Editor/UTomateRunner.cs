//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The runner which executes an automation plan, provides timing information and progress.
    /// </summary>
    public class UTomateRunner
    {
        private static UTomateRunner runner;
        private UTContext context;
        private IEnumerator enumerator;
        private TimeSpan expectedTime;
        private bool planLookupDone;
        private UTAutomationPlan lastPlan;

        public delegate void RunnerStarted();

        public event RunnerStarted OnRunnerStarted;

        public delegate void RunnerFinished(bool canceled, bool failed);

        public event RunnerFinished OnRunnerFinished;

        private bool assembliesWereReloaded;
        private bool playerSettingsRunInBackgroundValue;

        private UTomateRunner()
        {
        }

        public static UTomateRunner Instance
        {
            get { return runner ?? (runner = new UTomateRunner()); }
        }

        /// <summary>
        /// Gets the entry that is currently being executed.
        /// </summary>
        public UTAutomationPlanEntry CurrentEntry
        {
            get
            {
                return context != null ? context.CurrentEntry : null;
            }
        }

        public UTAutomationPlan CurrentPlan { get; private set; }

        public bool CancelRequested
        {
            get { return context != null && context.CancelRequested; }
            set {
                if (context != null)
                {
                    context.CancelRequested = value;
                } 
            }
        }

        public UTAutomationPlan LastPlan
        {
            get
            {
                if (planLookupDone)
                {
                    return lastPlan;
                }
                var lastRunPlan = UTPreferences.LastRunPlan;
                lastPlan = UTomate.UTAutomationPlanByName(lastRunPlan);
                planLookupDone = true;
                return lastPlan;
            }
        }

        /// <summary>
        /// Requests to run a certain plan. Only one plan can run at a time.
        /// </summary>
        /// <param name='planToRun'>
        /// The plan to run.
        /// </param>
        /// <param name='contextToUse'>
        /// Context in which the plan should run.
        /// </param>
        public void RequestRun(UTAutomationPlan planToRun, UTContext contextToUse)
        {
#if UTOMATE_DEMO
		// when developing utomate demo locally we want to allow our own plans, so we set another flag which
		// removes this restriction for us.
#if !UTOMATE_DEVELOPMENT_MODE
		if (UTomate.CheckPlanCountExceeded()) {
			return;
		}
#endif
#endif

            if (IsRunning)
            {
                throw new InvalidOperationException("The runner is currently busy. Use IsRunning to check if the runner is busy, before invoking this.");
            }
            CurrentPlan = planToRun;
            context = contextToUse;
            float expectedTimeInSeconds;
            PlanWasRunBefore = UTStatistics.GetExpectedRuntime(planToRun, out expectedTimeInSeconds);
            expectedTime = TimeSpan.FromSeconds(expectedTimeInSeconds);
            UTPreferences.LastRunPlan = planToRun.name;
            planLookupDone = false;

            if (OnRunnerStarted != null)
            {
                OnRunnerStarted();
            }
            // save all changes to assets before run.
            AssetDatabase.SaveAssets();
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 // VR: [5.0,5.4]
            EditorApplication.SaveAssets();
#endif

            // Lock Assembly Reloading
            assembliesWereReloaded = false;
            EditorApplication.LockReloadAssemblies();

            // keep stuff running in background
            // this will overwrite playersettings implicitely, therefore we store the previous value here..
            playerSettingsRunInBackgroundValue = PlayerSettings.runInBackground;
            Application.runInBackground = true;
            // and set it back here... so we don't fuck up the settings that were set before running utomate.
            PlayerSettings.runInBackground = playerSettingsRunInBackgroundValue;
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= ContinueRunning;
            EditorApplication.update += ContinueRunning;
        }

        public TimeSpan ExpectedTime
        {
            get { return expectedTime; }
        }

        public bool PlanWasRunBefore { get; private set; }

        public bool IsRunning
        {
            get
            {
                return CurrentPlan != null || context != null;
            }
        }

        public DateTime StartTime { get; private set; }

        private void ContinueRunning ()
		{
			if (CurrentPlan == null || context == null) {
				return;
			}
		
			// don't reload assemblies during build, this will break some builds
			if (EditorApplication.isCompiling && !assembliesWereReloaded )
			{
				assembliesWereReloaded = true;
              //  EditorApplication.LockReloadAssemblies();
				Debug.LogWarning("Detected project recompile while executing automation. Please make sure that the Unity window stays in focus, otherwise uTomate might be interrupted.");
			}
		
			try {
				if (enumerator == null) {
					// clear selection to avoid exceptions for missing resources due to reload
					Selection.objects = new UnityEngine.Object[0];
				
					StartTime = DateTime.Now;
					Debug.Log ("uTomate - Running plan: " + CurrentPlan.name);
					enumerator = CurrentPlan.Execute (context);
				} else {
					if (!enumerator.MoveNext ()) {
						CleanUp ();
					}
				}
			} catch (UTFailBuildException e) {
				context.Failed = true;
				e.LogToConsole ();
			    var planName = CurrentPlan != null ? CurrentPlan.name : "temporary plan";
				Debug.LogError ("Fail: Execution of plan " + planName + " failed.");
				CleanUp ();	
			} catch (Exception e) {
				context.Failed = true;
			    var lastAction = context.Me as UnityEngine.Object;
				Debug.LogException(e, lastAction);
				CleanUp ();
			}

		}

        private void CleanUp()
        {
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= ContinueRunning;

            UTils.ClearAsyncProgressBar();
            var cancelled = context.CancelRequested;
            var failed = context.Failed;
            context = null;
            enumerator = null;

            var endTime = DateTime.Now;
            var duration = endTime - StartTime;
            if (!cancelled && !failed)
            { // recording cancelled runs will greatly diminish the accuracy of the statistics, so don't record them
                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(CurrentPlan)))
                { // don't record statistics for temporary plans
                    UTStatistics.RecordRuntime(CurrentPlan, (float)duration.TotalSeconds);
                }
            }
            CurrentPlan = null;
            Debug.Log("Automation finished in " + UTils.FormatTime(duration, false) + ".");
            if (cancelled)
            {
                Debug.LogWarning("Run was canceled by user.");
            }
            if (failed)
            {
                Debug.LogError("Run failed with error.");
            }

            if (OnRunnerFinished != null)
            {
                OnRunnerFinished(cancelled, failed);
            }

            // now check if the player settings were modified by an action, in this case we don't reset them
            var newBackgroundSetting = PlayerSettings.runInBackground;
            Application.runInBackground = false;

            // and reset it back if it wasn't so we don't fuck up what was set before..
            if (newBackgroundSetting == playerSettingsRunInBackgroundValue)
            {
                // not modified by an action, in this case reset it
                PlayerSettings.runInBackground = playerSettingsRunInBackgroundValue;
            }

            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.Refresh(); // make sure updates are shown in the editor.
        }
    }
}
