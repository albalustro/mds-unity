using System.Collections;
using System.Collections.Generic;
using MDS.Actions;
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

    public bool selfTarget;

    public bool useMemorizedGameObjectAsTarget;

    [FullInspector.InspectorHideIf("selfTarget")]
    public GameObject target;

    public float duration;

	public bool useMemorizedGOPosition;
    public bool local;
    public Vector3 destination;

    public LeanTweenType easeType;

    public bool destroyOnComplete;

    private bool _animationComplete;

    public override IEnumerator Execute()
    {
        yield return base.Execute();

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

        _animationComplete = false;

        LTDescr move = null;

        switch(animationType)
        {
		case TransformAnimationType.Position:
			if (local)
				destination = target.transform.position + destination;

			if (useMemorizedGOPosition)
				destination = MemorizeMe.m_transform;
			
                move = LeanTween.move(target, destination, duration);
                break;

            case TransformAnimationType.Rotation:
                if(local)
                    destination = target.transform.rotation.eulerAngles + destination;
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
