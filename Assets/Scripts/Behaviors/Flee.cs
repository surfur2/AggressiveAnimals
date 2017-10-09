using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : BehaviorBase {

    public Transform fleeTarget;

    protected override void CalculateHeading()
    {
        var desiredVelocity = (gameObject.transform.position - fleeTarget.position).normalized * BotController.maxSpeed;
        desiredSteeringHeading = desiredVelocity - new Vector3(myRigidBody.velocity.x, myRigidBody.velocity.y, 0.0f);
    }
}
