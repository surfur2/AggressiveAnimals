using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSensorySystem : MonoBehaviour {

    struct ObstacleDistance
    {
        public GameObject obstacle;
        public float distance;

        public ObstacleDistance(GameObject _obstacle, float _distance)
        {
            obstacle = _obstacle;
            distance = _distance;
        }
    }

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
    private Transform currentClosestObstacle;
    private List<GameObject> obstaclesInFrontOfPlayer = new List<GameObject>();

    private int currentHp;
    public Color damageColor;
    public Color healColor;
    private Color originalColor;
    private const float FLASH_TIME = 0.5f;

    // Behaviors
    private Arrival arrivalBehavior;
    private Persue persueBehavior;
    private Attack attackBehavior;
    private Eat eatBehavior;
    public Flee fleeEnemyBehavior;
    public Flee avoidObstacleBehavior;

    // Controller for applying force to the bot
    private BotController botActuators;

    // The one true heading we will send to the bot controller
    private Vector3 heading;

    // My components
    private SpriteRenderer mySpriteRend;

    private void Start()
    {
        arrivalBehavior = gameObject.GetComponent<Arrival>();
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
        if (!fleeEnemyBehavior.isInhibited)
        {
            if (isScared)
            {
                var closestEnemy = FindClosestEnemy();
                currentEnemyFleeing = closestEnemy == null ? null : closestEnemy.transform;

                fleeEnemyBehavior.SetTarget(currentEnemyFleeing);

                // We are scared and activly fleeing
                if (currentEnemyFleeing != null && WithinFleeRadius(currentEnemyFleeing.position))
                {
                    fleeEnemyBehavior.InhibitLowerBehaviors();
                    heading = fleeEnemyBehavior.GetBehaviorHeading();
                }
                // We are scared but not activly fleeing
                else
                {
                    fleeEnemyBehavior.ResumeLowerBehaviors();
                }
            }
            // We are no longer scared
            else
            {
                fleeEnemyBehavior.ResumeLowerBehaviors();
            }
        }

        // Level 3 -> Persue enemies, be aggresive, and attack.
        if (!persueBehavior.isInhibited)
        {
            GameObject newTarget = null;
            // Resources have depleted.
            if (currentResourcePersued == null)
            {
                newTarget = FindClosestEnemy();
            }
            // Find the closes enemy to our resource.
            else
            {
                newTarget = FindClosestEnemyToResource(currentResourcePersued);
            }

            // Only set new target if it is not the same as old target
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

        // Level 1 -> Eat food if it is in radius.
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
       if (!avoidObstacleBehavior.isInhibited)
        {
            var currentObstacle = FindClosestObstacle();
            avoidObstacleBehavior.SetTarget(currentObstacle);

            if (currentObstacle != null)
            {
                heading = avoidObstacleBehavior.GetBehaviorHeading();
            }
        }
    }

    private Resource FindNearestFood()
    {
        var closestResource = GameManager.instance.FindClosestResource(transform.position);
        if (closestResource != null)
            return closestResource.GetComponent<Resource>();

        return null;
    }

    private GameObject FindClosestEnemy()
    {
        return GameManager.instance.FindClosestEnemyToResource(transform, gameObject);
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

    private Transform FindClosestObstacle ()
    {
        float minDistance = 100.0f;
        Transform minTransform = null;
        for (int i = 0; i < obstaclesInFrontOfPlayer.Count; i++)
        {
            var obstacle = obstaclesInFrontOfPlayer[i];
            var distance = (transform.position - obstacle.transform.position).magnitude;
                
            if (distance < minDistance)
            {
                minDistance = distance;
                minTransform = obstacle.transform;
            }
        }

        return minTransform;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "obstacle")
            if (!obstaclesInFrontOfPlayer.Contains(other.gameObject))
                obstaclesInFrontOfPlayer.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "obstacle")
            if (obstaclesInFrontOfPlayer.Contains(other.gameObject))
                obstaclesInFrontOfPlayer.Remove(other.gameObject);
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
