using UnityEngine;

public static class Vector3Extensions {
    public static Vector3 Clamp (this Vector3 val, Vector3 min, Vector3 max) {
        val.x = Mathf.Clamp (val.x, min.x, max.x);
        val.y = Mathf.Clamp (val.y, min.y, max.y);
        val.z = Mathf.Clamp (val.z, min.z, max.z);
        return val;
    }
}