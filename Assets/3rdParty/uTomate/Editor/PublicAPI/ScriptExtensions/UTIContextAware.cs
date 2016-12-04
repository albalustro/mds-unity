//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate.API
{
    /// <summary>
    /// Interface for script extensions that are aware of the context. Script extensions implementing this interface
    /// will get the current context injected after they have been constructed. Since the context will be in a bootstrapping
    /// phase when it is injected, Script extensions must not make any assumptions on the content of the current context.
    /// </summary>
    public interface UTIContextAware
    {
        UTContext Context
        {
            set;
        }
    }
}
