using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    Rigidbody2D rb;
    float offset;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        offset = transform.position.x % 1f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (rb.velocity.x == 0)
        {
            //transform.position = new Vector3(transform.position.Round(Vector3.right * offset).x, transform.position.y);
        }
        var v = rb.velocity;
        v.x = 0;
        rb.velocity = v;
    }
}
