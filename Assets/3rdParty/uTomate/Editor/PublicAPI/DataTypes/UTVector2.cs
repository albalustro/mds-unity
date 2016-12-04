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
    public class UTVector2 : UTProperty<Vector2>
    {
        [SerializeField]
        private Vector2 val;

        public override Vector2 Value
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
