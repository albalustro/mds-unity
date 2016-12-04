//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate.API
{
    using System;
    using UnityEngine;
    using UObject = UnityEngine.Object;

    [Serializable]
    public class UTUnityObjectBase<T> : UTProperty<T> where T : UObject
    {
        [SerializeField]
        private T propertyValue;

        public override T Value
        {
            get
            {
                return propertyValue;
            }
            set
            {
                propertyValue = value;
            }
        }
    }
}
