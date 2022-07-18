using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTileCollisionController : MonoBehaviour
{
    PlayerController player;
    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.OnTriggerEnter2DFirstTile(collision);
    }
}
