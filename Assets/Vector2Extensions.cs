using UnityEngine;

public static class Vector2Extensions {
    public static Vector2 DegreeToVector2 (float angle) {
        float x = Mathf.Cos (angle);
        float y = Mathf.Sin (angle);

        return new Vector2 (x, y);
    }
}