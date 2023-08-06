using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Exts
{
    public static Transform RewritePosition(this Transform value, float? x = null, float? y = null, float? z = null)
    {
        value.position = value.position.Rewrite(x, y, z);
        return value;
    }

    public static Vector3 Rewrite(this Vector3 value, float? x=null, float? y=null, float? z=null)
    {
        if(x != null)
        {
            value.x = x.Value;
        }
        if (y != null)
        {
            value.y = y.Value;
        }
        if (z != null)
        {
            value.z = z.Value;
        }
        return value;
    }
}
