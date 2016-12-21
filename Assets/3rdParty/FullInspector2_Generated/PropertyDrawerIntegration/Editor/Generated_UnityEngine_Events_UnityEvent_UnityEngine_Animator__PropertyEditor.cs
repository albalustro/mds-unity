using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(UnityEngine.Events.UnityEvent<UnityEngine.Animator>))]
    public class Generated_UnityEngine_Events_UnityEvent_UnityEngine_Animator__PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_UnityEngine_Events_UnityEvent_UnityEngine_Animator__MonoBehaviourStorage, Generated_UnityEngine_Events_UnityEvent_UnityEngine_Animator__NoGenerics> {
        public override bool CanEdit(Type type) {
            return typeof(UnityEngine.Events.UnityEvent<UnityEngine.Animator>).IsAssignableFrom(type);
        }
    }
}
