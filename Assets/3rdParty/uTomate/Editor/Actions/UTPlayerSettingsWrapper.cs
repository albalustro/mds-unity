//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

using UnityEngine;

namespace AncientLightStudios.uTomate
{
    using System;
    using UnityEditor;

    public class UTPlayerSettingsWrapper : IDisposable
    {
        private readonly SerializedObject serializedObject;

        public UTPlayerSettingsWrapper()
        {
            serializedObject = new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));
        }

        public static string GetPropertyNameForBuildTarget(BuildTargetGroup buildTarget, string propertyName)
        {
            return UTInternalCall.InvokeStatic("UnityEditor.PlayerSettings", "GetPropertyNameForBuildTarget", buildTarget, propertyName) as string;
        } 

        public void SetBool(string name, bool value)
        {
            serializedObject.FindProperty(name).boolValue = value;
        }

        public bool GetBool(string name)
        {
            return serializedObject.FindProperty(name).boolValue;
        }


        public void SetInt(string name, int value)
        {
            serializedObject.FindProperty(name).intValue = value;
        }

        public int GetInt(string name)
        {
            return serializedObject.FindProperty(name).intValue;
        }
        
        
        public void SetEnum(string name, Enum value) {
            SetInt(name, Convert.ToInt32(value));
        }
        
        public T GetEnum<T>(string name) {
            int value = GetInt(name);
            return (T) Enum.ToObject(typeof(T), value);
        }
        
		public void SetFloat(string name, float value)
		{
			serializedObject.FindProperty(name).floatValue = value;
		}
		
		public float GetFloat(string name)
		{
			return serializedObject.FindProperty(name).floatValue;
		}

        public void SetString(string name, string value)
        {
            serializedObject.FindProperty(name).stringValue = value;
        }

        public string GetString(string name)
        {
            return serializedObject.FindProperty(name).stringValue;
        }

		public void SetColor(string name, Color value)
		{
			serializedObject.FindProperty(name).colorValue = value;
		}
		
		public Color GetColor(string name)
		{
			return serializedObject.FindProperty(name).colorValue;
		}

        public void SetObject(string name, UnityEngine.Object obj)
        {
            serializedObject.FindProperty(name).objectReferenceValue = obj;
        }

        public UnityEngine.Object GetObject(string name)
        {
            return serializedObject.FindProperty(name).objectReferenceValue;
        }

        public void SetArray(string name, UnityEngine.Object[] values)
        {
            var property = serializedObject.FindProperty(name);
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

        }

        public UnityEngine.Object[] GetArray(string name)
        {
            var property = serializedObject.FindProperty(name);
            var result = new UnityEngine.Object[property.arraySize];
            for (var i = 0; i < property.arraySize; i++)
            {
                result[i] = property.GetArrayElementAtIndex(i).objectReferenceValue;
            }
            return result;
        }

        public void DoWith(string name, Action<SerializedProperty> function)
        {
            var property = serializedObject.FindProperty(name);
            function(property);
        }

        public void Dispose()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
