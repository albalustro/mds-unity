using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(UnityEngine.Events.UnityEvent<float>))]
    public class Generated_UnityEngine_Events_UnityEvent_float__PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_UnityEngine_Events_UnityEvent_float__MonoBehaviourStorage, Generated_UnityEngine_Events_UnityEvent_float__NoGenerics> {
        public override bool CanEdit(Type type) {
            return typeof(UnityEngine.Events.UnityEvent<float>).IsAssignableFrom(type);
        }
    }
}
