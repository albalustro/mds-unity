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
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// FieldWrapper which works on array members which are not type of UTProperty.
    /// </summary>
    public class UTPlainArrayMemberWrapper : UTFieldWrapper
    {
        private GUIContent label;
        private Type arrayType;
        private object[] array;
        private int arrayIndex;
        private FieldInfo fieldInfo;

        public void SetUp(GUIContent label, FieldInfo fieldInfo, object[] array, int arrayIndex, UTInspectorRendererDelegate rendererDelegate)
        {
            this.label = label;
            this.arrayType = fieldInfo.FieldType.GetElementType();
            this.array = array;
            this.arrayIndex = arrayIndex;
            this.fieldInfo = fieldInfo;
            RendererDelegate = rendererDelegate;
        }

        public string Expression
        {
            get
            {
                throw new NotImplementedException("Array fields do not support expressions.");
            }
            set
            {
                throw new NotImplementedException("Array fields do not support expressions.");
            }
        }

        public bool IsAsset
        {
            get
            {
                return arrayType.IsSubclassOf(typeof(UnityEngine.Object));
            }
        }

        public Type AssetType
        {
            get
            {
                if (!IsAsset)
                {
                    throw new NotImplementedException("Array field does not contain an asset type.");
                }
                return arrayType;
            }
        }

        public bool IsBool
        {
            get
            {
                return arrayType == typeof(bool);
            }
        }

        public bool IsString
        {
            get
            {
                return arrayType == typeof(string);
            }
        }

        public bool IsFloat
        {
            get
            {
                return arrayType == typeof(float);
            }
        }

        public bool IsInt
        {
            get
            {
                return arrayType == typeof(int);
            }
        }

        public bool IsEnum
        {
            get
            {
                return arrayType.IsEnum;
            }
        }

        public bool IsColor
        {
            get
            {
                return arrayType == typeof(Color);
            }
        }

        public UnityEngine.GUIContent Label
        {
            get
            {
                return label;
            }
        }

        public Type FieldType
        {
            get
            {
                return fieldInfo.FieldType;
            }
        }

        public string FieldName
        {
            get
            {
                return fieldInfo.Name;
            }
        }

        public bool SupportsExpressions
        {
            get
            {
                return false; // plain properties do not support expressions.
            }
        }

        public bool UseExpression
        {
            get
            {
                throw new NotImplementedException("Plain fields do not support expressions.");
            }
            set
            {
                throw new NotImplementedException("Plain fields do not support expressions.");
            }
        }

        public object Value
        {
            get
            {
                return array[arrayIndex];
            }
            set
            {
                array[arrayIndex] = value;
            }
        }

        public UTInspectorHint InspectorHint
        {
            get
            {
                return UTInspectorHint.GetFor(fieldInfo);
            }
        }

        public UTInspectorRendererDelegate RendererDelegate { get; private set; }
    }
}
