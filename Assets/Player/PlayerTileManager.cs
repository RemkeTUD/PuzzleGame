using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTileManager : MonoBehaviour
{
    [System.Serializable]
    public class TileCondition
    {
        public Vector2 nextDir, prevDir;
        public Sprite sprite;
        public bool rotateable;
        public bool flippable;

    }

    public List<TileCondition> conditions;

    public (float angle, bool flippedX, bool flippedY, Sprite sprite) getTileForDirections(Vector2 nextDir, Vector2 prevDir)
    {
        for(float angle = 0; angle < 360; angle += 90)
        {
            var possibleConditions = conditions.Where(c =>
                c.nextDir == (Vector2)(Quaternion.Euler(0, 0, angle) * nextDir) &&
                c.prevDir == (Vector2)(Quaternion.Euler(0, 0, angle) * prevDir));

            bool useFlipX = false;
            bool useFlipY = false;
            if(possibleConditions.Count() == 0)
            {
                useFlipX = (angle / 90f) % 2 == 0;
                useFlipY = (angle / 90f) % 2 == 1;
                possibleConditions = conditions.Where(
                    c => c.flippable &&
                    c.nextDir == (Vector2)(Quaternion.Euler(0, 0, angle) * nextDir.ScaleBy(new Vector2(-1,1))) &&
                    c.prevDir == (Vector2)(Quaternion.Euler(0, 0, angle) * prevDir.ScaleBy(new Vector2(-1, 1))));
            }

            if (possibleConditions.Count() > 0)
            {
                var conToUse = possibleConditions.First();
                return (angle, useFlipX, useFlipY, conToUse.sprite);
            }
        }
        return (0, false, false, conditions[0].sprite);
    }
}
