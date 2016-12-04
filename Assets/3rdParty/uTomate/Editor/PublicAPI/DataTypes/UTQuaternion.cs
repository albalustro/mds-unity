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
    public class UTQuaternion : UTProperty<Quaternion>
    {
        [SerializeField]
        private Quaternion val;

        public override Quaternion Value
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
    }
}
