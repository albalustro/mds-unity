using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(UnityEngine.Events.UnityEvent<UnityEngine.Behaviour>))]
    public class Generated_UnityEngine_Events_UnityEvent_UnityEngine_Behaviour__PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_UnityEngine_Events_UnityEvent_UnityEngine_Behaviour__MonoBehaviourStorage, Generated_UnityEngine_Events_UnityEvent_UnityEngine_Behaviour__NoGenerics> {
        public override bool CanEdit(Type type) {
            return typeof(UnityEngine.Events.UnityEvent<UnityEngine.Behaviour>).IsAssignableFrom(type);
        }
    }
}
