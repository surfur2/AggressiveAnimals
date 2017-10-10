using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    private float percentRemaining;

    [SerializeField]
    private Transform healthRemaing;
    [SerializeField]
    private int animalNumber;

    private float initialSize;

    private void Start()
    {
        GetComponentInChildren<TextMesh>().text = animalNumber.ToString();
        initialSize = healthRemaing.localScale.x;

        EventManager.instance.animalHealthChange.AddListener(UpdateGui);
    }
 
    private void UpdateGui(int _animalNumber, float percentHealth)
    {
        if (_animalNumber == animalNumber)
        {
            percentRemaining = percentHealth;

            healthRemaing.localScale = new Vector3(percentRemaining * initialSize, healthRemaing.localScale.y, healthRemaing.localScale.z);

        }
    }
}
