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
public class UTVector4 : UTProperty<Vector4>
{
    [SerializeField]
    private Vector4 val;

    public override Vector4 Value
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

