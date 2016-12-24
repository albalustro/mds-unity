using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Actions;
using MDS.Gameplay.DragDrop;
using UnityEngine;

public class TransformAnimationAction : BaseAction
{

    public enum TransformAnimationType
    {
        Position,
        Scale,
        Rotation,
    }
		
    public TransformAnimationType animationType;

    private bool IsPositionAnim { get { return animationType == TransformAnimationType.Position; } }
    private bool IsScaleAnim { get { return animationType == TransformAnimationType.Scale; } }
    private bool IsRotationAnim { get { return animationType == TransformAnimationType.Rotation; } }

    [InspectorHideIf("Hide_selfTarget")]
    public bool selfTarget;
    private bool Hide_selfTarget { get { return useMemorizedGameObjectAsTarget || _useSlotContentAsTarget != null || target != null; } }

    [InspectorHideIf("Hide_useMemorizedGameObjectAsTarget")]
    public bool useMemorizedGameObjectAsTarget;
    private bool Hide_useMemorizedGameObjectAsTarget { get { return selfTarget || _useSlotContentAsTarget != null || target != null; } }


    [SerializeField, InspectorHideIf("Hide_useSlotContentAsTarget")]
    private DropGroupSlot _useSlotContentAsTarget;
    private bool Hide_useSlotContentAsTarget { get { return selfTarget || useMemorizedGameObjectAsTarget || target!=null; } }

    [InspectorHideIf("HideTarget")]
    public GameObject target;
    private bool HideTarget { get { return selfTarget || useMemorizedGameObjectAsTarget || _useSlotContentAsTarget!=null; } }

    public float duration;

    [FullInspector.InspectorHideIf("HideDestination")]
    public bool local;

    [FullInspector.InspectorHideIf("Hide_useSelfPositionAsDestination")]
    public bool useSelfPositionAsDestination;

    [FullInspector.InspectorHideIf("Hide_useMemorizedGOAsDestination")]
    public bool useMemorizedGOAsDestination;

    [FullInspector.InspectorHideIf("Hide_useThisTransformAsDestination")]
    public Transform useThisTransformAsDestination;

    [FullInspector.InspectorHideIf("HideDestination")]
    public Vector3 destination;
    private bool HideDestination{ get { return useSelfPositionAsDestination || useMemorizedGOAsDestination || useThisTransformAsDestination!=null; } }
    private bool Hide_useSelfPositionAsDestination { get { return useMemorizedGOAsDestination || useThisTransformAsDestination != null; } }
    private bool Hide_useMemorizedGOAsDestination { get { return useSelfPositionAsDestination  || useThisTransformAsDestination != null; } }
    private bool Hide_useThisTransformAsDestination { get { return useSelfPositionAsDestination || useMemorizedGOAsDestination; } }

    public LeanTweenType easeType;

    public bool destroyOnComplete;

    private bool _animationComplete;

    public override IEnumerator Execute()
    {
        yield return base.Execute();


        // ALTERANDO O TARGET DA ANIMACAO
        if(selfTarget)
            target = _corotineHolder.gameObject;

        if (useMemorizedGameObjectAsTarget)
        {
            target = MemorizeMe.MemorizedGameObject;
            if (target==null)
            {
                Debug.LogError("Tentando animar objeto memorizado sem haver um..");
                yield break;
            }
        }

        if (_useSlotContentAsTarget!=null)
        {
            target = _useSlotContentAsTarget.draggableReference.gameObject;
            if (target==null)
            {
                Debug.LogError("Tentando animar conteudo de slot sem haver um..");
                yield break;
            }
        }

        // ALTERANDO O DESTINO DA ANIMACAO
        Transform tmp = null;
        if(useSelfPositionAsDestination || useMemorizedGOAsDestination || useThisTransformAsDestination != null)
        {
            local = false;
            
            if(useSelfPositionAsDestination)
            {
                tmp = _corotineHolder.gameObject.transform;
            }

            if(useMemorizedGOAsDestination)
            {
                tmp = MemorizeMe.MemorizedGameObject.transform;
            }

            if(useThisTransformAsDestination != null)
            {
                tmp = useThisTransformAsDestination;
            }

            switch(animationType)
            {
                case TransformAnimationType.Position:
                    destination = tmp.position;
                    break;
                case TransformAnimationType.Scale:
                    destination = tmp.lossyScale;
                    break;
                case TransformAnimationType.Rotation:
                    destination = tmp.rotation.eulerAngles;
                    break;
                default:
                    break;
            }
        }


        _animationComplete = false;

		LTDescr move = null;

        switch(animationType)
        {
			case TransformAnimationType.Position:
				if (local)
					move = LeanTween.move(target, target.transform.position + destination, duration);
				else
					move = LeanTween.move(target, destination, duration);
                break;

            case TransformAnimationType.Rotation:
                if(local)
					move = LeanTween.rotate(target, target.transform.rotation.eulerAngles + destination, duration);
				else
					move = LeanTween.rotate(target, destination, duration);
                break;

            case TransformAnimationType.Scale:
                move = LeanTween.scale(target, destination, duration);
                break;
            default:
                break;
        }

        move.setEase(easeType);
        move.setDestroyOnComplete(destroyOnComplete);
        move.setOnComplete(() => _animationComplete = true);

        if(waitFinish)
            yield return new WaitWhile(() => _animationComplete != true);

    }


}
