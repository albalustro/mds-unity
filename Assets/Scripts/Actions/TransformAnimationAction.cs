using System;
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
        Rotation
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


    [InspectorTooltip("Todos os targets vão usar o mesmos parametros e serão animados simultaneamente")]
    public GameObject[] multipleTargets;


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

        

        switch(animationType)
        {
            case TransformAnimationType.Position:

                if (target!=null)
                    ApplyPositionAnimation(target);

                if(multipleTargets != null && multipleTargets.Length > 0)
                {
                    foreach(var t in multipleTargets)
                    {
                        if(t != null)
                            ApplyPositionAnimation(t);
                    }
                }

                break;

            case TransformAnimationType.Rotation:

                if (target!=null)
                    ApplayRototationAnimation(target);

                if(multipleTargets != null && multipleTargets.Length > 0)
                {
                    foreach(var t in multipleTargets)
                    {
                        if(t != null)
                            ApplayRototationAnimation(t);
                    }
                }

                break;

            case TransformAnimationType.Scale:

                if(target != null)
                    ApplayScaleAnimation(target);

                if(multipleTargets != null && multipleTargets.Length > 0)
                {
                    foreach(var t in multipleTargets)
                    {
                        if(t != null)
                            ApplayScaleAnimation(t);
                    }
                }
                

                break;

            default:
                break;
        }


        if(waitFinish)
            yield return new WaitWhile(() => _animationComplete != true);

    }

    private void ApplayScaleAnimation(GameObject go)
    {
        LeanTween.scale(go, destination, duration)
                .setEase(easeType)
                .setDestroyOnComplete(destroyOnComplete)
                .setOnComplete(() => _animationComplete = true);
    }

    private void ApplayRototationAnimation(GameObject go)
    {
        if(local)
            destination = go.transform.rotation.eulerAngles + destination;

        LeanTween.rotate(go, destination, duration)
                .setEase(easeType)
                .setDestroyOnComplete(destroyOnComplete)
                .setOnComplete(() => _animationComplete = true);
    }

    private void ApplyPositionAnimation(GameObject go)
    {
        if(local)
            destination = go.transform.position + destination;

        LeanTween.move(go, destination, duration)
                .setEase(easeType)
                .setDestroyOnComplete(destroyOnComplete)
                .setOnComplete(() => _animationComplete = true);
    }
}
