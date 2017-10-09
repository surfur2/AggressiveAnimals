using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrival : BehaviorBase {

    public Transform seekTarget;
    public float slowingDistance;

    protected override void CalculateHeading()
    {
        var targetOffset = seekTarget.position - gameObject.transform.position;
        var rampedSpeed = (targetOffset.magnitude / slowingDistance) * BotController.maxSpeed;
        var desiredVelocity = Mathf.Min(rampedSpeed, BotController.maxSpeed) * targetOffset;
        desiredSteeringHeading = desiredVelocity - new Vector3(myRigidBody.velocity.x, myRigidBody.velocity.y , 0.0f);
    }
}
