using UnityEngine;
using UnityEngine.EventSystems;

namespace Playmove
{
    public class PYDragMouse : PYDrag
    {
        public DragType DragOnPlane;
        public bool UseLimitArea;
        public Vector2 LimitArea;
        public float DeadArea = 0.5f;
        public Transform TargetToMove;

        private Transform _target;
        private Vector2 _pointerWorldPosition;
        private Vector2 _deltaPos;
        private bool _insideDeadArea;

        protected override void Start()
        {
            base.Start();

            _target = TargetToMove == null ? OwnTransform : TargetToMove;
            UseLimitArea = DragOnPlane != DragType.Free && UseLimitArea;
        }

        protected override void DragBeginAction(PointerEventData eventData)
        {
            _insideDeadArea = true;
            _pointerWorldPosition = eventData.pointerCurrentRaycast.worldPosition;
            _deltaPos = new Vector2(_target.position.x - _pointerWorldPosition.x, _target.position.y - _pointerWorldPosition.y);
            UpdateDrag(_pointerWorldPosition + _deltaPos);
        }

        protected override void DraggingAction(PointerEventData eventData)
        {
            if (_insideDeadArea)
            {
                Vector3 dragDeadArea = (Vector3)_pointerWorldPosition - eventData.pointerCurrentRaycast.worldPosition;

                if (DragOnPlane == DragType.Horizontal)
                {
                    if (Mathf.Abs(dragDeadArea.x) < DeadArea)
                        return;
                }

                if (DragOnPlane == DragType.Vertical)
                {
                    if (Mathf.Abs(dragDeadArea.y) < DeadArea)
                        return;
                }
            }

            _insideDeadArea = false;

            _pointerWorldPosition = eventData.pointerCurrentRaycast.worldPosition;
            UpdateDrag(_pointerWorldPosition + _deltaPos);
        }

        private void UpdateDrag(Vector3 newPosition)
        {
            if (OutsideLimitArea(newPosition))
                return;

            switch (DragOnPlane)
            {
                case DragType.Free:
                    _target.position = newPosition;
                    break;

                case DragType.Horizontal:
                    _target.position = new Vector3(newPosition.x, _target.position.y);
                    break;

                case DragType.Vertical:
                    _target.position = new Vector3(_target.position.x, newPosition.y);
                    break;
            }
        }

        private bool OutsideLimitArea(Vector3 dragPosition)
        {
            if (!UseLimitArea || DragOnPlane == DragType.Free)
                return false;

            if (DragOnPlane == DragType.Horizontal)
            {
                if (dragPosition.x < LimitArea.x || dragPosition.x > LimitArea.y)
                    return true;
            }

            if (DragOnPlane == DragType.Vertical)
            {
                if (dragPosition.y < LimitArea.x || dragPosition.y > LimitArea.y)
                    return true;
            }

            return false;
        }
    }
}