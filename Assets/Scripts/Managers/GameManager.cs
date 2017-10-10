using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField]
    private GameObject[] resources;
    [SerializeField]
    private List<GameObject> animals;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        EventManager.instance.animalDied.AddListener(AnimalDied);
    }

    public GameObject FindClosestResource(Vector3 positionToCalculate)
    {
        float currentMinDistance = 100.0f;
        GameObject currentMinTransform = null;

        foreach (GameObject resource in resources)
        {
            float currentDistanceAway = (resource.transform.position - positionToCalculate).magnitude;
            if (currentDistanceAway < currentMinDistance)
            {
                currentMinDistance = currentDistanceAway;
                currentMinTransform = resource;
            }
        }

        return currentMinTransform;
    }

    public GameObject FindClosestEnemyToResource (Transform resource, GameObject me)
    {
        float currentMinDistance = 100.0f;
        GameObject currentMinTransform = null;

        foreach (GameObject animal in animals)
        {  
            float currentDistanceAway = (animal.transform.position - resource.position).magnitude;
            if (currentDistanceAway < currentMinDistance && animal != me)
            {
                currentMinDistance = currentDistanceAway;
                currentMinTransform = animal;
            }
        }

        return currentMinTransform;
    }

    private void AnimalDied (GameObject animal)
    {
        animals.Remove(animal);
        Destroy(animal);
    }
}
