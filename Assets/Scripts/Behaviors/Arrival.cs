using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrival : BehaviorBase {

    private Transform target;
    public float slowingDistance;

    protected override void CalculateBehavior()
    {
        if (target != null)
        {
            var targetOffset = target.position - gameObject.transform.position;
            var rampedSpeed = (targetOffset.magnitude / slowingDistance) * BotController.maxSpeed;
            var desiredVelocity = Mathf.Min(rampedSpeed, BotController.maxSpeed) * targetOffset;
            desiredSteeringHeading = desiredVelocity - new Vector3(myRigidBody.velocity.x, myRigidBody.velocity.y, 0.0f);
        }
    }

    public void SetTarget(Resource _target)
    {
        target = null;

        if (_target != null)
            target = _target.transform;
    }
}
