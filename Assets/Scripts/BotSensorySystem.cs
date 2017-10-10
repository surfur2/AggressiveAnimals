using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSensorySystem : MonoBehaviour {

    // Cap how often we are updating behavior
    public float capUpdateCycle;
    private float timerToNextUpdateCycle;

    // Values for determining behaviors
    public float radiusForAggression;
    public int startingHp;

    // State of the animal
    private Transform currentResourcePersued;
    private Rigidbody2D currentEnemyPersued;
    private int currentHp;

    // Behaviors
    private Arrival arrivalBehavior;
    private Flee fleeBehavior;
    private Persue persueBehavior;
    private Attack attackBehavior;

    // Controller for applying force to the bot
    private BotController botActuators;

    // The one true heading we will send to the bot controller
    private Vector3 heading;

    private void Start()
    {
        arrivalBehavior = gameObject.GetComponent<Arrival>();
        fleeBehavior = gameObject.GetComponent<Flee>();
        persueBehavior = gameObject.GetComponent<Persue>();
        attackBehavior = gameObject.GetComponent<Attack>();

        botActuators = gameObject.GetComponent<BotController>();
        heading = new Vector3(0.0f, 0.0f, 0.0f);
        timerToNextUpdateCycle = 0.0f;
        currentHp = startingHp;
    }

    private void Update()
    {
        if (timerToNextUpdateCycle <= 0.0f)
        {
            DetermineSubsumptionBehavior();
            botActuators.SetHeading(heading);
        }
        else
        {
            timerToNextUpdateCycle -= Time.deltaTime;
        }
    }

    private void DetermineSubsumptionBehavior()
    {
        // Level 3 -> Flee from enemies when at low hp

        // Level 2 -> Persue enemies and be aggresive
        if (!persueBehavior.isInhibited)
        {
            var newTarget = FindClosestEnemyToResource(currentResourcePersued);
            if (currentEnemyPersued != newTarget)
            {
                currentEnemyPersued = newTarget;
                persueBehavior.SetTarget(currentEnemyPersued);
                persueBehavior.ResumeLowerBehaviors();

                attackBehavior.SetTarget(currentEnemyPersued.GetComponent<BotSensorySystem>());
            }

            if (currentEnemyPersued != null && WithinRadiusOfFood(transform.position) && WithinRadiusOfFood(currentEnemyPersued.position))
            {
                persueBehavior.InhibitLowerBehaviors();
                heading = persueBehavior.GetBehaviorHeading();
            }
            else
            {
                persueBehavior.ResumeLowerBehaviors();
            }
        }

        // Level 1 -> Seek food
        if (!arrivalBehavior.isInhibited)
        {
            currentResourcePersued = FindNearestFood();
            arrivalBehavior.SetTarget(currentResourcePersued);
            heading = arrivalBehavior.GetBehaviorHeading();
        }

      
        // Level 0 -> Avoid obstacles

    }

    private Transform FindNearestFood()
    {
        return GameManager.instance.FindClosestResource(transform.position).transform;
    }

    private Rigidbody2D FindClosestEnemyToResource(Transform currentResourcePersued)
    {
        Rigidbody2D closestEnemyRB2D = null;
        if (currentResourcePersued != null)
        {
            var closestEnemy = GameManager.instance.FindClosestEnemyToResource(currentResourcePersued, gameObject);
            if (closestEnemy != null)
                closestEnemyRB2D = closestEnemy.GetComponent<Rigidbody2D>();
        }

        return closestEnemyRB2D;
    }

    private bool WithinRadiusOfFood(Vector3 positionWithinRadius)
    {
        if (positionWithinRadius != null && currentResourcePersued != null)
        {
            return (positionWithinRadius - currentResourcePersued.position).magnitude < radiusForAggression ? true : false;
        }

        return false;
    }

    public void TakeDamage (int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            EventManager.instance.animalDied.Invoke(gameObject);
            this.enabled = false;
        }
    }
}
