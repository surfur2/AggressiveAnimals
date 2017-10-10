﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public static EventManager instance;

    [HideInInspector]
    public AnimalDied animalDied;
    [HideInInspector]
    public AnimalHealthChange animalHealthChange;
    [HideInInspector]
    public ResourceDepleted resourceDepleted;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        animalDied = new AnimalDied();
        animalHealthChange = new AnimalHealthChange();
        resourceDepleted = new ResourceDepleted();
    }
}
