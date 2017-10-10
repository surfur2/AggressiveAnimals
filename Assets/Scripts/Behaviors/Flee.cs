using UnityEngine;

public class Flee : BehaviorBase {

    private Transform target;

    protected override void CalculateBehavior()
    {
        if (target != null)
        {
            var desiredVelocity = (gameObject.transform.position - target.position).normalized * BotController.maxSpeed;
            desiredSteeringHeading = desiredVelocity - new Vector3(myRigidBody.velocity.x, myRigidBody.velocity.y, 0.0f);
        }
    }


    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}
