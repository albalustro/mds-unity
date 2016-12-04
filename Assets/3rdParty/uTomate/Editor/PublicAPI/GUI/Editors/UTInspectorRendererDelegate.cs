//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate.API
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Interface for delegate classes which want to override the default rendering process of <see clink="UTInspectorRenderer"/>.
    /// </summary>
    public interface UTInspectorRendererDelegate
    {
        /// <summary>
        /// Determines whether or not the given field should be visible.
        /// </summary>
        /// <returns>
        /// <c>Undetermined</c> of the delegate cannot decide whether or not the field should be visible. This will
        /// delegate the visibility decision to the inspector renderer. <c>Visible</c> if the field should be visible.
        /// <c>Invisible</c> if the field should not be visible. Return <c>Undetermined</c> for all fields not
        /// specifically handled by your implementation.
        /// </returns>
        /// <param name='field'>
        /// The field to check.
        /// </param>
        UTVisibilityDecision IsVisible(FieldInfo field);

        /// <summary>
        /// Determines, which options should be available when this field is renderer. This is only called for fields
        /// which actually have options, e.g. enum fields.
        /// </summary>
        /// <param name="field">the field to check</param>
        /// <returns>an array of available options. Returns null if this function cannot determine the available options. In this case
        /// the renderer will use a default set of options</returns>
        string[] AllowedValues(string field);

        /// <summary>
        /// Draws an array member using the given wrapper. Usually this only needs to be implemented if the delegate
        /// should control how array members are drawn. If you simply want to do a renderer for a certain
        /// type, do it in <c>DrawProperty</c> instead as <c>DrawProperty</c> will be called by the default
        /// implementation for rendering parts of an array.
        /// </summary>
        /// <returns>
        /// True if the delegate has rendered this field, false if the default implementation should render it.
        /// </returns>
        /// <param name='wrapper'>
        /// The wrapper around this field.
        /// </param>
        /// <param name='deleteMember'>
        /// If set to true the array member will be deleted after the rendering method finishes. Use this
        /// for driving a delete button in the rendering output.
        /// </param>
        [Obsolete("This has no function anymore and will be removed in future uTomate version. Please use the UTIPropertyRenderer interface instead.")]
        bool DrawArrayMember(UTFieldWrapper wrapper, out bool deleteMember);

        /// <summary>
        /// Draws the property specified by the given wrapper. There is always a BeginHorizontal/EndHorizontal wrapped
        /// around this by the default implementation. If the given property is a part of an array
        /// </summary>
        /// <returns>
        /// True if the delegate has rendered this field, false if the default implementation should render it.
        /// </returns>
        /// <param name='wrapper'>
        /// The wrapper describing the field.
        /// </param>
        [Obsolete("This has no function anymore and will be removed in future uTomate version. Please use the UTIPropertyRenderer interface instead.")]
        bool DrawProperty(UTFieldWrapper wrapper);
    }
}
