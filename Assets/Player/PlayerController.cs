using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;

    public float maxSpeed;
    public GameObject tilePrefab;
    public Transform tilesContainer;
    public Transform eyes;
    public PlayerTileManager playerTileManager;
    public List<BaseTileController> allBaseTiles = new List<BaseTileController>();
    public SwipeController swipeController;

    public int maxLength = 4;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTileManager = GetComponent<PlayerTileManager>();
        swipeController = GetComponent<SwipeController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow) || swipeController.upSwipe)
        {
            processTileInput(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || swipeController.leftSwipe)
        {
            processTileInput(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || swipeController.rightSwipe)
        {
            processTileInput(Vector2.right);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || swipeController.downSwipe)
        {
            processTileInput(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.Space) || swipeController.tap)
        {
            DestroyFirstTile();
        }
    }

    void DestroyFirstTile()
    {

        if (tilesContainer.childCount > 2)
            Destroy(getFirstTile().gameObject);
    }

    private void FixedUpdate()
    {
        Debug.Log(isGrounded() ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic);
        //rb.bodyType = isGrounded() ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;

        if (!isGrounded() && Time.timeSinceLevelLoad > 0.1f)
        {
            transform.Translate(Vector2.down * 4f * Time.fixedDeltaTime);
        }
        if(isGrounded() && roundIt)
        {
            transform.position = transform.position.Round(Vector3.zero);
        }


        if(Input.GetKey(KeyCode.D))
        {
            speed = Mathf.Lerp(speed, maxSpeed, 0.2f * Time.fixedDeltaTime * 60f);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            speed = Mathf.Lerp(speed, -maxSpeed, 0.2f * Time.fixedDeltaTime * 60f);
        }
        else
        {
            speed = Mathf.Lerp(speed, 0, 0.2f * Time.fixedDeltaTime * 60f);
        }

        var v = rb.velocity;
        v.x = speed;
        rb.velocity = v;
    }

    public Vector3 getMidPoint()
    {
        
        return getWorldSpaceBounds().center;
    }

    public Bounds getWorldSpaceBounds()
    {
        Bounds bounds = new Bounds(allBaseTiles[0].transform.position, Vector3.zero);

        foreach (var tile in allBaseTiles)
        {
            bounds.Encapsulate(tile.transform.position);
        }
        return bounds;
    }

    void processTileInput(Vector2 direction)
    {
        if (getLastBaseTile().pivot.transform.localScale != Vector3.one || !isGrounded())
            return;
        if (getLastBaseTile().dir == -direction)
            DestroyLastTile();
        else if (!blockedThroughOtherDirection(direction) && !blocksItself(direction))
        {
            addTile(direction);
            if (allBaseTiles.Count > maxLength)
                DestroyFirstTile();
        }
    }

    void DestroyLastTile()
    {
        if (allBaseTiles.Count <= 2)
            return;
        eyes.parent = null;
        DestroyImmediate(getLastTile().gameObject);
        eyes.parent = getLastTile();
        eyes.localPosition = Vector3.zero;
        eyes.rotation = Quaternion.identity;
    }
    bool roundIt = true;
    void addTile(Vector2 direction)
    {

        var prevTile = getLastBaseTile();

        if (hasSomethingInFront(direction))
        {
            //transform.position -= (Vector3)direction * 1f;
            StartCoroutine(shiftToDirection(-direction));
        }
        if(roundIt)
            transform.position = transform.position.Round(Vector3.zero);
        var prevCellPos = getLastBaseTile().cellPos;
        var t = Instantiate(tilePrefab, (Vector2)getLastTile().position + direction.normalized, Quaternion.identity, tilesContainer);
        t.transform.localRotation = Quaternion.identity;
        var bt = t.GetComponent<BaseTileController>();
        bt.dir = direction;
        bt.cellPos = (prevCellPos + direction);
        bt.player = this;
        eyes.parent = bt.square;
        eyes.localPosition = Vector2.zero;
        eyes.rotation = Quaternion.identity;

        bt.prevBaseTile = prevTile;
        prevTile.nextBaseTile = bt;

    }

    IEnumerator shiftToDirection(Vector2 direction)
    {
        roundIt = false;
        Vector2 totalShift = Vector2.zero;

        while(totalShift.magnitude < 1)
        {
            Vector3 amount = direction * Mathf.Min(0.5f*Time.deltaTime * 60f, 1f - totalShift.magnitude);
            totalShift += (Vector2)amount;
            transform.position += amount;
            yield return null;

                
        }
        roundIt = true;
    }

    Transform getLastTile()
    {
        return tilesContainer.GetChild(tilesContainer.childCount - 1);
    }
    Transform getFirstTile()
    {
        return tilesContainer.GetChild(0);
    }
    BaseTileController getLastBaseTile()
    {
        return getLastTile().gameObject.GetComponent<BaseTileController>();
    }

    BaseTileController getBaseTileForCellPos(Vector2 cellPos)
    {
        foreach(var baseTile in allBaseTiles)
        {
            if (baseTile.cellPos == cellPos)
                return baseTile;
        }
        return null;
    }

    bool blocksItself(Vector2 dir)
    {

        var result = getBaseTileForCellPos(getLastBaseTile().cellPos + dir) != null;
        //Debug.Log("Blocked Somethin in front: " + result);
        return result;

    }

    bool hasSomethingInFront(Vector2 dir)
    {
        var h = Physics2D.Raycast(getLastTile().position, dir, 0.55f);
        var rb = h.collider?.gameObject.GetComponent<Rigidbody2D>();
        var result = h && (rb == null || rb.bodyType == RigidbodyType2D.Static || rbCollisionChain(rb, dir));

        //Debug.Log("Blocked Somethin in front: " + result);
        return result;
    }

    bool rbCollisionChain(Rigidbody2D rb, Vector2 dir)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        var h = rb.Cast(dir, hits, 0.05f);
        var otherRB = hits.Count > 0 ? hits[0].rigidbody : null;
        return h > 0 && (otherRB == null || otherRB.bodyType == RigidbodyType2D.Static || rbCollisionChain(otherRB, dir));
    }

    bool blockedThroughOtherDirection(Vector2 dir)
    {
        List<RaycastHit2D> res = new List<RaycastHit2D>();
        var castNum = rb.Cast(-dir, res, 0.55f);
        var otherrb = res.Count>0 ? (res?[0].collider?.gameObject.GetComponent<Rigidbody2D>()) : null;

        var result = hasSomethingInFront(dir) && (castNum > 0) && (otherrb == null || otherrb.bodyType == RigidbodyType2D.Static);

       // Debug.Log("Blocked Other Dir: " + result);

        return result;
    }
    bool isGrounded()
    {
        List<RaycastHit2D> res = new List<RaycastHit2D>();
        foreach(var col in tilesContainer.gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            var h = Physics2D.BoxCast((Vector2)col.transform.position + Vector2.up * 0.1f, Vector2.one * 0.95f, 0, Vector2.down, 0.15f);
            if(h && (h.collider.attachedRigidbody == null || h.collider.attachedRigidbody != rb))
            {
                Debug.Log(h.collider.gameObject.name);
                res.Add(h);
            }
        }
        return res.Count > 0;
    }
}
