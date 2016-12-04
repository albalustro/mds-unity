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
    public class UTRect : UTProperty<Rect>
    {
        [SerializeField]
        private Rect val;

        public override Rect Value
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
