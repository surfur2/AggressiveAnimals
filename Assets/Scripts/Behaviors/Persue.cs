using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Persue behavior of a target witha  slowing effect as you approach the target.
/// Note this behavior is currently not very accurate at predicting turning.
/// </summary>
public class Persue : BehaviorBase {

    public Rigidbody2D seekTarget;
    public float slowingDistance;
    // Hard codded value used to determine how far in the distance we are predicting for the target.
    public float timePredictor;

    protected override void CalculateHeading()
    {
        var currentDistanceFromTarget = (seekTarget.transform.position - gameObject.transform.position).magnitude;
        var predictedDistanceTraveled = new Vector3(seekTarget.velocity.x, seekTarget.velocity.y, 0.0f) * (timePredictor * currentDistanceFromTarget);
        var futurePositionForTarget = seekTarget.transform.position + predictedDistanceTraveled;

        var targetOffset = futurePositionForTarget - gameObject.transform.position;
        var rampedSpeed = (targetOffset.magnitude / slowingDistance) * BotController.maxSpeed;
        var desiredVelocity = Mathf.Min(rampedSpeed, BotController.maxSpeed) * targetOffset;

        desiredSteeringHeading = desiredVelocity - new Vector3(myRigidBody.velocity.x, myRigidBody.velocity.y, 0.0f);
    }
}
