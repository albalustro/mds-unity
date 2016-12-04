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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UObject = UnityEngine.Object;

    /// <summary>
    /// uTomate's main window.
    /// </summary>
    public class UTMainWindow : EditorWindow, CUListContextMenuListener, CUListExecutionListener
    {
        private bool showHidden;
        private bool debugMode;
        private bool clearConsole;
        private bool returnToPlanListAfterRun;
        private CUListData listData = new CUListData(false);
        private List<UTAutomationPlan> plans;
        private List<UTAutomationPlan> visiblePlans;
        private readonly UTAutomationPlanListItemRenderer renderer;
        private int currentWindow;
        private readonly string[] toolBarLabels = { "Plans", "Runner", "Project Properties", "Editor Properties" };
        private PropertyEditorState projectPropertyEditorState = new PropertyEditorState();
        private PropertyEditorState editorPropertyEditorState = new PropertyEditorState();
        private Vector2 scrollPosition;

        private UTMainWindow()
        {
            listData.ContextMenuHandler = this;
            listData.ExecutionListener = this;
            renderer = new UTAutomationPlanListItemRenderer();
#if UTOMATE_DEMO
			var windowTitle = "uTomate Demo";
#else
			var windowTitle = "uTomate";
#endif

#if UNITY_5_0 // VR: [5.0]
			title = windowTitle;
#else
			titleContent = new GUIContent(windowTitle);
#endif
        }

        [MenuItem("Window/uTomate/Help/Manual", false, 450)]
        public static void Manual()
        {
            UTils.OpenHelpFile("uTomate-Manual", "http://www.ancientlightstudios.com/utomate/documentation/manual.html");
        }

        [MenuItem("Window/uTomate/Main Window", false, 100)]
        public static void Init()
        {
            var window = GetWindow<UTMainWindow>(
#if UTOMATE_DEMO
			"uTomate Demo",
#else
"uTomate",
#endif
 UTInternalCall.GetType("UnityEditor.InspectorWindow"));
            window.minSize = new Vector2(400, 250);
            window.Show();
        }

        [MenuItem("Assets/Run Automation Plan", true)]
        private static bool ValidateRunAutomationPlan()
        {
            return Selection.activeObject is UTAutomationPlan;
        }

        [MenuItem("Assets/Run Automation Plan", false, 700)]
        private static void RunAutomationPlan()
        {
            var plan = Selection.activeObject as UTAutomationPlan;
            plan.Execute();
        }

        [MenuItem("Window/uTomate/Run Automation Plan... %k", false, 110)]
        private static void SelectAndRunAutomationPlan()
        {
            SelectAutomationPlan("Execute automation plan", ExecuteSelectedPlan);
        }

        /// <summary>
        /// Callback for the popup window
        /// </summary>
        /// <param name='plan'>
        /// the plan to execute
        /// </param>
        private static void ExecuteSelectedPlan(UTAutomationPlan plan)
        {
            plan.Execute();
        }

        [MenuItem("Window/uTomate/Edit Automation Plan... #%k", false, 115)]
        private static void SelectAndEditAutomationPlan()
        {
            SelectAutomationPlan("Edit automation plan", EditSelectedPlan);
        }

        /// <summary>
        /// Callback for the popup window
        /// </summary>
        /// <param name='plan'>
        /// the plan to edit
        /// </param>
        private static void EditSelectedPlan(UTAutomationPlan plan)
        {
            UTAutomationPlanWindow.EditAutomationPlan(plan);
        }

        [MenuItem("Window/uTomate/Locate Automation Plan... &%k", false, 120)]
        private static void SelectAndLocateAutomationPlan()
        {
            SelectAutomationPlan("Locate automation plan", LocateSelectedPlan);
        }

        /// <summary>
        /// Callback for the popup window
        /// </summary>
        /// <param name='plan'>
        /// the plan to locate
        /// </param>
        private static void LocateSelectedPlan(UTAutomationPlan plan)
        {
            EditorGUIUtility.PingObject(plan);
        }

        private static void SelectAutomationPlan(string caption, UTAutomationPlanPopup.AutomationPlanSelected callback)
        {
            List<UTAutomationPlan> plans;
            List<UTAutomationPlan> visiblePlans;
            LoadAutomationPlans(out plans, out visiblePlans);
            UTAutomationPlanPopup.ShowDialog(caption, plans, visiblePlans, callback);
        }

        public void HandleContextMenu(int index)
        {
            if (index != -1)
            {
                var planList = showHidden ? plans : visiblePlans;
                var plan = planList[index];
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Run"), false, ExecutePlan, plan);
                menu.AddItem(new GUIContent("Edit"), false, EditPlan, plan);
                menu.AddItem(new GUIContent("Locate"), false, LocatePlan, plan);
                menu.ShowAsContext();
            }
        }

        public bool HandleExecution(List<int> indices)
        {
            // we dont have multiselect
            if (indices != null && indices.Count == 1)
            {
                var planList = showHidden ? plans : visiblePlans;
                ExecutePlan(planList[indices[0]]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Callback for the context menu
        /// </summary>
        /// <param name='plan'>
        /// the plan to execute
        /// </param>
        private static void ExecutePlan(object plan)
        {
            if (plan is UTAutomationPlan)
            {
                ExecuteSelectedPlan(plan as UTAutomationPlan);
            }
        }

        /// <summary>
        /// Callback for the context menu
        /// </summary>
        /// <param name='plan'>
        /// the plan to edit
        /// </param>
        private static void EditPlan(object plan)
        {
            if (plan is UTAutomationPlan)
            {
                EditSelectedPlan(plan as UTAutomationPlan);
            }
        }

        /// <summary>
        /// Callback for the context menu
        /// </summary>
        /// <param name='plan'>
        /// the plan to locate
        /// </param>
        private static void LocatePlan(object plan)
        {
            if (plan is UTAutomationPlan)
            {
                LocateSelectedPlan(plan as UTAutomationPlan);
            }
        }

        public void OnEnable()
        {
            showHidden = UTPreferences.ShowHiddenPlansInRunner;
            debugMode = UTPreferences.DebugMode;
            clearConsole = UTPreferences.ClearConsoleBeforeStart;
            returnToPlanListAfterRun = UTPreferences.ReturnToPlanListAfterPlanIsRun;
            UTomateRunner.Instance.OnRunnerStarted += OnRunnerStarted;
            UTomateRunner.Instance.OnRunnerFinished += OnRunnerFinished;
            RefreshInternals();
        }

        public void OnDisable()
        {
            UTomateRunner.Instance.OnRunnerStarted -= OnRunnerStarted;
            UTomateRunner.Instance.OnRunnerFinished -= OnRunnerFinished;
        }

        public void OnFocus() 
        {
            RefreshInternals();
        }

        public void OnHierarchyChange()
        {
            RefreshInternals();
        }

        public void OnProjectChange()
        {
            RefreshInternals();
        }

        private void RefreshInternals()
        {
            LoadAutomationPlans(out plans, out visiblePlans);
            Repaint();
        }

        private static void LoadAutomationPlans(out List<UTAutomationPlan> allPlans, out List<UTAutomationPlan> visiblePlans)
        {
            allPlans = UTomate.AllUTAutomationPlans();
            visiblePlans = allPlans.FindAll(x => !x.hideInExecutionWindow);
        }

        public void OnGUI()
        {
            var oldWindow = currentWindow;
            currentWindow = GUILayout.Toolbar(oldWindow, toolBarLabels, EditorStyles.toolbarButton);

            if (oldWindow != currentWindow)
            {
                GUIUtility.keyboardControl = 0; // reset input so textfields get updated
            }

            EditorGUILayout.BeginVertical();

            if (currentWindow == 0)
            {
                DrawActions();
            }

            if (currentWindow == 1)
            {
                DrawRunner();
            }

            if (currentWindow == 2)
            {
                DrawProjectProperties();
            }

            if (currentWindow == 3)
            {
                DrawEditorProperties();
            }

            EditorGUILayout.EndVertical();
        }

        private void OnRunnerStarted()
        {
            currentWindow = 1;
            Repaint();
        }

        private void OnRunnerFinished(bool canceled, bool failed)
        {
            if (returnToPlanListAfterRun)
            {
                currentWindow = 0;
                Repaint();
            }
        }

        private void DrawProjectProperties()
        {
            EditorGUILayout.HelpBox("The properties you set up here will be available to all actions of the current project.", MessageType.None, true);

            EditorGUI.BeginChangeCheck();

            var properties = UTomate.ProjectProperties;
            projectPropertyEditorState = DrawPropertyEditor(properties.Properties(), projectPropertyEditorState,
                properties.AddProperty, properties.PropertyExists, properties.DeleteProperty, properties);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(properties);
            }
        }

        private void DrawEditorProperties()
        {
            EditorGUILayout.HelpBox("The properties you set up here will be available to all actions in all projects.", MessageType.None, true);
            editorPropertyEditorState = DrawPropertyEditor(UTEditorProperties.Properties(), editorPropertyEditorState,
                UTEditorProperties.AddInternal, UTEditorProperties.PropertyExists, UTEditorProperties.DeleteProperty, null);
        }

        private delegate void DoAdd(string propertyName);

        private delegate bool PropertyExists(string propertyName);

        private delegate void DoDelete(UTConfigurableProperty property);

        private PropertyEditorState DrawPropertyEditor(IEnumerable<UTConfigurableProperty> properties, PropertyEditorState state,
            DoAdd doAdd, PropertyExists propertyExists, DoDelete doDelete, UObject undoTarget)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            state.newPropertyName = EditorGUILayout.TextField("Name", state.newPropertyName);
            GUI.enabled = !string.IsNullOrEmpty(state.newPropertyName) && UTContext.IsValidPropertyName(state.newPropertyName) && !propertyExists(state.newPropertyName);
            if (GUILayout.Button("Add", UTEditorResources.ExpressionButtonStyle))
            {
                if (undoTarget != null)
                {
                    Undo.RecordObject(undoTarget, "Add Property");
                }
                doAdd(state.newPropertyName);
                state.newPropertyName = "";
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var property in properties)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();

                if (!property.UseExpression)
                {
                    if (property.IsPrivate)
                    {
                        property.Value = EditorGUILayout.PasswordField(new GUIContent(property.Name, property.Name), property.Value);
                    }
                    else
                    {
						property.Value = EditorGUILayout.TextField(new GUIContent(property.Name, property.Name), property.Value);
                    }
                    if (property.SupportsPrivate)
                    {
                        if (GUILayout.Button(property.IsPrivate ? UTEditorResources.PrivateIcon : UTEditorResources.PublicIcon, UTEditorResources.DeleteButtonStyle))
                        {
                            property.IsPrivate = !property.IsPrivate;
                            GUIUtility.keyboardControl = 0; // make textfield update.
                        }
                    }
                }
                else
                {
					property.Expression = EditorGUILayout.TextField(new GUIContent(property.Name, property.Name), property.Expression);
                }

                if (GUILayout.Button(UTEditorResources.DeleteIcon, UTEditorResources.DeleteButtonStyle))
                {
                    if (undoTarget != null)
                    {
                        Undo.RecordObject(undoTarget, "Remove property");
                    }
                    doDelete(property);
                    GUIUtility.ExitGUI();
                }

                var oldExpression = property.UseExpression;
                property.UseExpression = GUILayout.Toggle(oldExpression, UTEditorResources.ExpressionButton, UTEditorResources.ExpressionButtonStyle);

                if (oldExpression != property.UseExpression)
                {
                    GUIUtility.keyboardControl = 0; // make textfield update.
                }

                EditorGUILayout.EndHorizontal();

                if (!property.UseExpression && property.Value.Contains("$"))
                {
                    EditorGUILayout.HelpBox(UTEditorResources.VariableWarningText, MessageType.Warning);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            return state;
        }

        private void DrawActions()
        {
            GUILayout.Label("Settings", UTEditorResources.TitleStyle);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            showHidden = EditorGUILayout.Toggle("Show hidden plans", showHidden);
            debugMode = EditorGUILayout.Toggle("Debug mode", debugMode);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            clearConsole = EditorGUILayout.Toggle("Clear console", clearConsole);
            returnToPlanListAfterRun = EditorGUILayout.Toggle("Return to plan list after run", returnToPlanListAfterRun);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                UTPreferences.DebugMode = debugMode;
                UTPreferences.ShowHiddenPlansInRunner = showHidden;
                UTPreferences.ClearConsoleBeforeStart = clearConsole;
                UTPreferences.ReturnToPlanListAfterPlanIsRun = returnToPlanListAfterRun;
            }

            var planList = showHidden ? plans : visiblePlans;
            listData = CUListControl.SelectionList(listData, planList, renderer, "Plans");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = listData.Selection.Count > 0 && !UTomateRunner.Instance.IsRunning;
            if (GUILayout.Button("Run Plan"))
            {
                currentWindow = 1;
                RunSelected();
                // some actions will reload the project and not exiting the gui here
                // will yield all kinds of funky exceptions because the gui is trying to draw on a state that
                // no longer exists.
                GUIUtility.ExitGUI();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void DrawRunner()
        {
            GUILayout.Label("Runner", UTEditorResources.TitleStyle);

            var runner = UTomateRunner.Instance;
            var lastPlan = runner.LastPlan;
            if (lastPlan != null && !runner.IsRunning)
            {
                var boxRect = EditorGUILayout.BeginHorizontal(UTEditorResources.RunAgainBoxStyle);
                EditorGUI.HelpBox(boxRect, "Last executed plan: " + lastPlan.name, MessageType.None);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(UTEditorResources.RunAgainIcon, UTEditorResources.RunAgainButtonStyle))
                {
                    UTomate.Run(lastPlan);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();

            DrawRunnerGui();

            if (!runner.IsRunning)
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUI.enabled = UTStatistics.HasStatistics();
                if (GUILayout.Button("Clear plan statistics"))
                {
                    UTStatistics.Clear();
                }
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                Repaint();
            }
        }

        private static void DrawRunnerGui()
        {
            var runner = UTomateRunner.Instance;

            EditorGUILayout.BeginVertical();
            if (runner.IsRunning && runner.CurrentPlan != null)
            {
                EditorGUILayout.LabelField("Running plan", runner.CurrentPlan.name);
                if (runner.CurrentEntry != null)
                {
                    EditorGUILayout.LabelField("Entry", runner.CurrentEntry.Label);
                }

                var difference = (DateTime.Now - runner.StartTime);
                string remainingTime;
                var progress = 0f;
                if (!runner.CancelRequested)
                {
                    if (runner.PlanWasRunBefore)
                    {
                        var expectedTime = runner.ExpectedTime;
                        if (difference.CompareTo(expectedTime) > 0)
                        {
                            remainingTime = "overtime: " + UTils.FormatTime(difference.Subtract(expectedTime), true);
                            progress = 1f;
                        }
                        else
                        {
                            remainingTime = "remaining: " + UTils.FormatTime(expectedTime.Subtract(difference), true);
                            if (expectedTime.TotalSeconds > 0)
                            {
                                progress = (float)(difference.TotalSeconds / expectedTime.TotalSeconds);
                            }
                        }
                    }
                    else
                    {
                        remainingTime = "running for: " + UTils.FormatTime(difference, true);
                    }
                }
                else
                {
                    remainingTime = "Cancelling current run...";
                }

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                var rect = GUILayoutUtility.GetRect(new GUIContent(""), UTEditorResources.ProgressBarStyle, GUILayout.ExpandWidth(true));
                EditorGUI.ProgressBar(rect, progress, remainingTime);

                GUI.enabled = !runner.CancelRequested;
                if (GUILayout.Button(UTEditorResources.CancelIcon, UTEditorResources.CancelButtonStyle))
                {
                    runner.CancelRequested = true;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Currently no plan is running.");
            }
            EditorGUILayout.EndVertical();
        }
	

        private void RunSelected()
        {
            ShowTab();
            var selectedItems = listData.GetSelectedItems(showHidden ? plans : visiblePlans);
            if (selectedItems.Count == 1)
            {
                UTomate.Run(selectedItems[0]);
            }
            RefreshInternals(); // build might change something, so better re-acquire all.
        }

        private class PropertyEditorState
        {
            public string newPropertyName = "";
        }
    }
}
