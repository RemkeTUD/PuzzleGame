using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTileController : MonoBehaviour
{
    public Transform pivot;
    public Transform square;
    public SpriteRenderer baseTileRenderer;
    public Vector2 dir;
    public Vector2 cellPos;
    public BaseTileController nextBaseTile, prevBaseTile;
    public Vector2 nextDir, prevDir;
    public PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        pivot.localPosition = -dir * 0.5f;
        square.position = transform.position;
        pivot.localScale = Vector3.zero;
        player.allBaseTiles.Add(this);
        setDirectionsAndUpdateSprite();
        if(prevBaseTile != null)
            prevBaseTile.setDirectionsAndUpdateSprite();
        if (nextBaseTile != null)
            nextBaseTile.setDirectionsAndUpdateSprite();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pivot.localScale = Vector3.MoveTowards(pivot.localScale, Vector3.one, 0.2f * Time.fixedDeltaTime * 60f);
    }

    void setDirectionsAndUpdateSprite()
    {
        if (nextBaseTile != null)
            nextDir = nextBaseTile.cellPos - cellPos;
        else
            nextDir = Vector2.zero;
        if (prevBaseTile != null)
            prevDir = prevBaseTile.cellPos - cellPos;
        else
            prevDir = Vector2.zero;

        var tile = player.playerTileManager.getTileForDirections(nextDir, prevDir);
        baseTileRenderer.sprite = tile.sprite;

        square.localRotation = Quaternion.Euler(0, 0, -tile.angle);
        baseTileRenderer.transform.localScale = (new Vector3(tile.flippedX ? -1 : 1, tile.flippedY ? -1 : 1, 1) * 0.2f);

    }

    private void OnDestroy()
    {
        if (prevBaseTile != null)
        {
            prevBaseTile.nextBaseTile = null;
            prevBaseTile.setDirectionsAndUpdateSprite();
        }
        if (nextBaseTile != null)
        {
            nextBaseTile.prevBaseTile = null;
            nextBaseTile.setDirectionsAndUpdateSprite();
        }
        player.allBaseTiles.Remove(this);
    }


}
