using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(MDS.Gameplay.DragDrop.MyUnityEvent))]
    public class Generated_MDS_Gameplay_DragDrop_MyUnityEvent_PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_MDS_Gameplay_DragDrop_MyUnityEvent_MonoBehaviourStorage, MDS.Gameplay.DragDrop.MyUnityEvent> {
        public override bool CanEdit(Type type) {
            return typeof(MDS.Gameplay.DragDrop.MyUnityEvent).IsAssignableFrom(type);
        }
    }
}
