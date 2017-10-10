using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorBase : MonoBehaviour {

    // Cap how often a behavior is updated so it does not hog too many resources.
    public float headingCalcDelay;
    private float timeToNextCalc;

    protected Vector3 desiredSteeringHeading;

    protected Rigidbody2D myRigidBody;
    protected BotSensorySystem mySensorSystem;

    [HideInInspector]
    public bool isInhibited;
    public int level;
    public List<BehaviorBase> subsumedBehaviors;
    public List<BehaviorBase> inhibitaedBehaviors;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        mySensorSystem = GetComponent<BotSensorySystem>();
        timeToNextCalc = 0.0f;
        isInhibited = false;
    }

    private void Update()
    {
        if (!isInhibited)
        {
            if (timeToNextCalc <= 0.0f)
            {
                CalculateBehavior();
                timeToNextCalc = headingCalcDelay;
            }

            timeToNextCalc -= Time.deltaTime;
        }
    }

    public Vector3 GetBehaviorHeading()
    {
        return desiredSteeringHeading;
    }

    public void SubsumeLowerBehaviors()
    {

    }

    // Block any heading calculations from lower behaviors
    public void InhibitLowerBehaviors()
    {
        foreach (BehaviorBase behavior in inhibitaedBehaviors)
        {
            behavior.isInhibited = true;
        }
    }

    // Resume heading calculations for lower behaviors.
    public void ResumeLowerBehaviors()
    {
        foreach (BehaviorBase behavior in inhibitaedBehaviors)
        {
            behavior.isInhibited = false;
        }
    }

    protected abstract void CalculateBehavior();
}
