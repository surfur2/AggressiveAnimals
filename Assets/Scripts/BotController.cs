using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour {

    // Scalars for Rynaolds behaviors
    // We can use rigidbody.mass
    //private float mass;
    public static float maxForce = 0.07f;
    public static float maxSpeed = 2.0f;

    // Vectors for Rynolds behaviors.
    // We can use gameobject.position
    // public Vector3 position; 

    // We can use myRigidbody.velocity
    //public Vector3 velocity;

    // N basis vectors. This is taken care of us by unity gameobject.forward
    /*public Vector3 newForward;
      public Vector3 newSide;
      public Vector3 newUp;*/

    private Rigidbody2D myRigidBody;
    private Vector3 _heading;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CalculateLocomotion(_heading);
    }

    // Reynolds calculation of an entities locomotion
    private void CalculateLocomotion(Vector3 heading)
    {
        // steeringForce = truncate (steering_direction, max_force)
        Vector2 steeringForce = heading;
        if (heading.magnitude > maxForce)
        {
            steeringForce = heading.normalized * maxForce;
        }

        // acceleration = steering_force / mass
        var acceleration = steeringForce / myRigidBody.mass;

        // velocity = truncate (velocity + acceleration, max_speed)
        var newVelocity = (myRigidBody.velocity + acceleration);
        if (newVelocity.magnitude > maxSpeed)
        {
            newVelocity = newVelocity.normalized * maxSpeed;
        }

        // postion = position + velocity is taken care of us by unity
        myRigidBody.velocity = newVelocity;

        // Reynolds calculates all necessary orientations in his model. Unity knows how to calculate the other aligments based on forward.
        // We also set the up direction for unity because it cooresponds to the "forward" vector described in Reynolds in 2D top down space.
        if (newVelocity.magnitude > 0.0f)
            gameObject.transform.up = newVelocity.normalized;
    }

    public void SetHeading(Vector3 heading)
    {
        _heading = heading;
    }
}
