using System.Collections;
using System.Collections.Generic;
using MDS.Actions;
using UnityEngine;

public class MoveAction : BaseAction {


    public bool selfTarget;

    [FullInspector.InspectorHideIf("selfTarget")]
    public GameObject target;

    public float duration;

    public Vector3 destination;

    public LeanTweenType easeType;

    public bool destroyOnComplete;

    public override IEnumerator Execute()
    {
        yield return base.Execute();

        if(selfTarget)
            target = _corotineHolder.gameObject;

        var anim = LeanTween.move(target, destination, duration).setEase(easeType);
        anim.destroyOnComplete = destroyOnComplete;

    }


}
