using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(UnityEngine.Events.UnityEvent<int>))]
    public class Generated_UnityEngine_Events_UnityEvent_int__PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_UnityEngine_Events_UnityEvent_int__MonoBehaviourStorage, Generated_UnityEngine_Events_UnityEvent_int__NoGenerics> {
        public override bool CanEdit(Type type) {
            return typeof(UnityEngine.Events.UnityEvent<int>).IsAssignableFrom(type);
        }
    }
}
