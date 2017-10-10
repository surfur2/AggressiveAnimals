using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryWrap : MonoBehaviour {

    public float onScreenOffset;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        var viewportPoint = Camera.main.WorldToViewportPoint(collision.gameObject.transform.position);

        if (viewportPoint.x > 1.0f)
        {
            collision.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.0f + onScreenOffset, viewportPoint.y, viewportPoint.z));
        }
        else if (viewportPoint.x < 0.0f)
        {
            collision.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.0f - onScreenOffset, viewportPoint.y, viewportPoint.z));
        }
        else if (viewportPoint.y > 1.0f)
        {
            collision.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(viewportPoint.x, 0.0f + onScreenOffset, viewportPoint.z));
        }
        else if (viewportPoint.y < 0.0f)
        {
            collision.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(viewportPoint.x, 1.0f - onScreenOffset, viewportPoint.z));
        }
    }
}
