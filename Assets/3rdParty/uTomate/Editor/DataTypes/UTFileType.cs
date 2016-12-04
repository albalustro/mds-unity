//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 // VR: 5.3
namespace AncientLightStudios.uTomate
{
    using System;
    using API;
    using UnityEngine;

    [Serializable]
    public class UTFileType : UTProperty<UTSetPlayerSettingsWindowsStoreAction.FileType>
    {
        [SerializeField]
        private UTSetPlayerSettingsWindowsStoreAction.FileType val;


        public override UTSetPlayerSettingsWindowsStoreAction.FileType Value
        {
            get { return val; } 
            set { val = value; }
        }

        protected override UTSetPlayerSettingsWindowsStoreAction.FileType CustomCast(object value)
        {
            var strings = value as string[];
            if (strings != null && strings.Length == 2)
            {
                var result = new UTSetPlayerSettingsWindowsStoreAction.FileType
                {
                    contentType = strings[0],
                    fileType = strings[1]
                };
                LogConversion(strings, result);
                return result;
            }
            return base.CustomCast(value);
        }
    }
}
#endif
