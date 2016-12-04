/// <summary>
/// Utility abstracting away the Undo system changes introduced in Unity 4.3.
/// </summary>
using System;
using UnityEditor;
using UObject = UnityEngine.Object;

public class CUUndoUtility
{
    [ObsoleteAttribute("Use Undo.RecordObject() instead.")]
    public static void RegisterUndo(UObject objectToUndo, string message)
    {
        Undo.RecordObject(objectToUndo, message);
    }

    [ObsoleteAttribute("Use Undo.RecordObjects() instead.")]
    public static void RegisterUndo(UObject[] objectsToUndo, string message)
    {
        // Unity 4.3+
        Undo.RecordObjects(objectsToUndo, message);
    }
}
