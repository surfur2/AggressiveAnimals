using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

    public int healthHealed;
    public int charges;
    public Color biteTakenColor;
    private Color initColor;
    private const float FLASH_TIME = 0.5f;

    private SpriteRenderer mySpriteRend;

    private void Start()
    {
        mySpriteRend = GetComponent<SpriteRenderer>();
        initColor = mySpriteRend.color;
    }

    public int TakeBite()
    {
        charges -= 1;
        StartCoroutine(FlashColor());
        if (charges == 0)
        {
            EventManager.instance.resourceDepleted.Invoke(gameObject);
        }

        this.enabled = false;

        return healthHealed;
    }

    private IEnumerator FlashColor()
    {
        mySpriteRend.color = biteTakenColor;
        yield return new WaitForSeconds(FLASH_TIME);
        mySpriteRend.color = initColor;
    }
}
