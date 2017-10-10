using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : BehaviorBase {

    private Resource target;

    public float radiusForEating;
    public float eatCooldown;
    private float lastAte;

    protected override void CalculateBehavior()
    {
        if (target != null)
        {
            if (lastAte < 0.0f && !isInhibited)
            {
                if (IsInEatingRadius())
                {
                    EatResource();
                }
            }
        }
        lastAte -= Time.deltaTime;
    }

    public void SetTarget(Resource _target)
    {
        target = _target;
    }

    private void EatResource()
    {
        var healthGained = target.TakeBite();
        mySensorSystem.RecoverHealth(healthGained);
        lastAte = eatCooldown;
    }

    private bool IsInEatingRadius()
    {
        return (target.gameObject.transform.position - transform.position).magnitude < radiusForEating ? true : false;
    }
}
