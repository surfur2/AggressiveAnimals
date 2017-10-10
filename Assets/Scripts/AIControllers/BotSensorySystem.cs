using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSensorySystem : MonoBehaviour {

    // Cap how often we are updating behavior
    public float capUpdateCycle;
    private float timerToNextUpdateCycle;

    public int botNumber;

    // Values for determining behaviors
    public float radiusForAggression;
    public float fleeRadius;
    public int startingHp;
    public float percentHealthToFlee;
    public float percentHealthToAttack;
    private float currentPercentHealth;
    private bool isScared;

    // State of the animal
    private Resource currentResourcePersued;
    private GameObject currentEnemyPersued;
    private Transform currentEnemyFleeing;

    private int currentHp;
    public Color damageColor;
    public Color healColor;
    private Color originalColor;
    private const float FLASH_TIME = 0.5f;

    // Behaviors
    private Arrival arrivalBehavior;
    private Flee fleeBehavior;
    private Persue persueBehavior;
    private Attack attackBehavior;
    private Eat eatBehavior;

    // Controller for applying force to the bot
    private BotController botActuators;

    // The one true heading we will send to the bot controller
    private Vector3 heading;

    // My components
    private SpriteRenderer mySpriteRend;

    private void Start()
    {
        arrivalBehavior = gameObject.GetComponent<Arrival>();
        fleeBehavior = gameObject.GetComponent<Flee>();
        persueBehavior = gameObject.GetComponent<Persue>();
        attackBehavior = gameObject.GetComponent<Attack>();
        eatBehavior = gameObject.GetComponent<Eat>();

        botActuators = gameObject.GetComponent<BotController>();

        mySpriteRend = GetComponent<SpriteRenderer>();
        GetComponentInChildren<TextMesh>().text = botNumber.ToString();

        heading = new Vector3(0.0f, 0.0f, 0.0f);
        timerToNextUpdateCycle = 0.0f;
        currentHp = startingHp;
        currentPercentHealth = 1.0f;
        isScared = false;
        originalColor = mySpriteRend.color;
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
        // Level 4 -> Flee from enemies when at low hp
        if (!fleeBehavior.isInhibited)
        {
            if (isScared)
            {
                currentEnemyFleeing = FindClosestEnemy();
                fleeBehavior.SetTarget(currentEnemyFleeing);

                // We are scared and activly fleeing
                if (currentEnemyFleeing != null && WithinFleeRadius(currentEnemyFleeing.position))
                {
                    fleeBehavior.InhibitLowerBehaviors();
                    heading = fleeBehavior.GetBehaviorHeading();
                }
                // We are scared but not activly fleeing
                else
                {
                    fleeBehavior.ResumeLowerBehaviors();
                }
            }
            // We are no longer scared
            else
            {
                fleeBehavior.ResumeLowerBehaviors();
            }
        }

        // Level 3 -> Persue enemies, be aggresive, and attack.
        if (!persueBehavior.isInhibited)
        {
            var newTarget = FindClosestEnemyToResource(currentResourcePersued);
            if (currentEnemyPersued != newTarget)
            {
                currentEnemyPersued = newTarget;

                persueBehavior.SetTarget(currentEnemyPersued);
                attackBehavior.SetTarget(currentEnemyPersued);
            }

            if (currentEnemyPersued != null && WithinRadiusOfFood(transform.position) && WithinRadiusOfFood(currentEnemyPersued.transform.position))
            {
                persueBehavior.InhibitLowerBehaviors();
                heading = persueBehavior.GetBehaviorHeading();
            }
            else
            {
                persueBehavior.ResumeLowerBehaviors();
            }
        }

        // Level 2 -> Eat food if it is in radius.
        if (!eatBehavior.isInhibited)
        {
            eatBehavior.SetTarget(currentResourcePersued);
        }

        // Level 1 -> Seek food and eat if within radius.
        if (!arrivalBehavior.isInhibited)
        {
            currentResourcePersued = FindNearestFood();
            arrivalBehavior.SetTarget(currentResourcePersued);
            heading = arrivalBehavior.GetBehaviorHeading();
        }

      
        // Level 0 -> Avoid obstacles

    }

    private Resource FindNearestFood()
    {
        var closestResource = GameManager.instance.FindClosestResource(transform.position);
        if (closestResource != null)
            return closestResource.GetComponent<Resource>();

        return null;
    }

    private Transform FindClosestEnemy()
    {
        var closestEnemy = GameManager.instance.FindClosestEnemyToResource(transform, gameObject);
        if (closestEnemy != null)
        {
            return closestEnemy.transform;
        }

        return null;
    }

    private GameObject FindClosestEnemyToResource(Resource currentResourcePersued)
    {
        if (currentResourcePersued != null)
            return GameManager.instance.FindClosestEnemyToResource(currentResourcePersued.transform, gameObject);

        return null;
    }

    private bool WithinRadiusOfFood(Vector3 positionWithinRadius)
    {
        if (currentResourcePersued != null)
            return (positionWithinRadius - currentResourcePersued.transform.position).magnitude < radiusForAggression ? true : false;

        return false;
    }

    private bool WithinFleeRadius (Vector3 positionWithinRadius)
    {
        return (positionWithinRadius - transform.position).magnitude < fleeRadius ? true : false;
    }

    public void TakeDamage (int damage)
    {
        currentHp -= damage;
        StartCoroutine(FlashDamage());
        currentPercentHealth = ((float)currentHp / (float)startingHp);
        EventManager.instance.animalHealthChange.Invoke(botNumber, currentPercentHealth);
        if (currentHp <= 0)
        {
            EventManager.instance.animalDied.Invoke(gameObject);
            this.enabled = false;
        }

        if (currentPercentHealth < percentHealthToFlee && !isScared)
        {
            isScared = true;
        }
    }

    public void RecoverHealth (int health)
    {
        currentHp += health;
        if (currentHp > startingHp)
            currentHp = startingHp;

        StartCoroutine(FlashHealing());
        currentPercentHealth = ((float)currentHp / (float)startingHp);
        EventManager.instance.animalHealthChange.Invoke(botNumber, currentPercentHealth);

        if (currentPercentHealth >= percentHealthToAttack)
        {
            isScared = false;
        }
    }

    private IEnumerator FlashDamage()
    {
        mySpriteRend.color = damageColor;
        yield return new WaitForSeconds(FLASH_TIME);
        mySpriteRend.color = originalColor;
    }

    private IEnumerator FlashHealing()
    {
        mySpriteRend.color = healColor;
        yield return new WaitForSeconds(FLASH_TIME);
        mySpriteRend.color = originalColor;
    }
}
