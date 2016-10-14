using UnityEngine;
using System.Collections;

public class ReturnDraggableToStartPosition : MDSBehaviour {

    public Transform traget;
    public float tragetDistanceOffset = 0.1f;
    public bool reachedTarget;
    public bool stopFollowAfterReachTarget;
    public float speed = 10;
    public bool followX, followY, followZ;
    private float xpos, ypos, zpos;
    public FollowMethod followMethod = FollowMethod.LERP;

    void Update()
    {
        if (traget == null)
            return;

        if (reachedTarget)
            if (stopFollowAfterReachTarget)
                return;

        if (followX)
            xpos = GetValue(transform.position.x, traget.position.x);
        else
            xpos = transform.position.x;

        if (followY)
            ypos = GetValue(transform.position.y, traget.position.y);
        else
            ypos = transform.position.y;

        if (followZ)
            zpos = GetValue(transform.position.z, traget.position.z);
        else
            zpos = transform.position.z;

        transform.position = new Vector3(xpos, ypos, zpos);

        if (Mathf.Abs(Vector3.Distance(traget.position, transform.position)) <= tragetDistanceOffset)
            this.reachedTarget = true;
    }

    public enum FollowMethod
    {
        LERP,
        SMOOTH_STEP,
    };

    private float GetValue(float currentValue, float targetValue)
    {
        float returnValue = 0;
        if (followMethod == FollowMethod.LERP)
            returnValue = Mathf.Lerp(currentValue, targetValue, speed * Time.deltaTime);
        else if (followMethod == FollowMethod.SMOOTH_STEP)
            returnValue = Mathf.SmoothStep(currentValue, targetValue, speed * Time.smoothDeltaTime);
        return returnValue;
    }
}
