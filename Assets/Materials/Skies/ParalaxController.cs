using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ParalaxController : MonoBehaviour
{
    public float depth;
    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = -cam.transform.position * depth;
    }
}
