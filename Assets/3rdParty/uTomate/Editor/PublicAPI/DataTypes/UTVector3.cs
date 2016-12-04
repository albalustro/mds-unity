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

    [Serializable]
    public class UTVector3 : UTProperty<Vector3>
    {
        [SerializeField]
        private Vector3 val;

        public override Vector3 Value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
            }
        }

        protected override Vector3 CustomCast(object val)
        {
            if (val is Quaternion)
            {
                var result = ((Quaternion)val).eulerAngles;
                LogConversion(val, result);
                return result;
            }
            return base.CustomCast(val);
        }
    }
}
