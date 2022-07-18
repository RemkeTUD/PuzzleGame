using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCameraController : MonoBehaviour
{
    PlayerController player;
    Camera cam;
    public float baseScale = 7f;
    public float extraScaleFactor = 0.1f;

    private void Start()
    {
        player = GameManager.player;
        cam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.getMidPoint() - Vector3.forward * 10f, 0.1f * Time.fixedDeltaTime * 60f);
        var b = player.getWorldSpaceBounds();
        float aspect = Screen.currentResolution.width / (float)Screen.currentResolution.height;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, baseScale + (Mathf.Max(b.size.x * (aspect), b.size.y * (1f / aspect))) * extraScaleFactor, 0.025f*Time.deltaTime*60f);
    }

}
