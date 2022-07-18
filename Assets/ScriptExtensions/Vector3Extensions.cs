using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 ScaleBy(this Vector2 vec, Vector2 scale)
    {
        return new Vector2(vec.x * scale.x, vec.y * scale.y);
    }
    public static Vector3 Round(this Vector3 vec, Vector3 offset)
    {
        return new Vector3(Mathf.Round(vec.x - offset.x), Mathf.Round(vec.y - offset.y), Mathf.Round(vec.z - offset.z)) + offset;
    }
}
