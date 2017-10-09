using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorBase : MonoBehaviour {

    // Cap how often a behavior is updated so it does not hog too many resources.
    public float headingCalcDelay;
    private float timeToNextCalc;

    protected Vector3 desiredSteeringHeading;

    protected Rigidbody2D myRigidBody;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        timeToNextCalc = 0.0f;
    }

    private void Update()
    {
        if (timeToNextCalc <= 0.0f)
        {
            CalculateHeading();
            timeToNextCalc = headingCalcDelay;
        }

        timeToNextCalc -= Time.deltaTime;
    }

    public Vector3 GetBehaviorHeading()
    {
        return desiredSteeringHeading;
    }

    protected abstract void CalculateHeading();
}
