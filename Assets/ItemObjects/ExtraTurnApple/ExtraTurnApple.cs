using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTurnApple : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<PlayerController>().maxLength++;
            Destroy(this.gameObject);
        }
    }
}
