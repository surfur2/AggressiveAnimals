using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSensorySystem : MonoBehaviour {

    public Transform currentTarget;

    private BotController botActuators;

    // Behaviors
    private Arrival arivalBehavior;
    private Flee fleeBehavior;
    private Persue persueBehavior;

    // The one true heading we will send to the bot controller
    private Vector3 heading;

    private void Start()
    {
        arivalBehavior = gameObject.GetComponent<Arrival>();
        fleeBehavior = gameObject.GetComponent<Flee>();
        persueBehavior = gameObject.GetComponent<Persue>();
        botActuators = gameObject.GetComponent<BotController>();
        heading = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void Update()
    {
        DetermineSubsumptionBehavior();

        botActuators.SetHeading(heading);
    }

    private void DetermineSubsumptionBehavior()
    {
       if (arivalBehavior.isActiveAndEnabled)
        {
            heading = arivalBehavior.GetBehaviorHeading();
        }
       else if (fleeBehavior.isActiveAndEnabled)
        {
            heading = fleeBehavior.GetBehaviorHeading();
        }
        else if (persueBehavior.isActiveAndEnabled)
        {
            heading = persueBehavior.GetBehaviorHeading();
        }
    }

}
