using UnityEngine;

[DisallowMultipleComponent]
public class JointIK : MonoBehaviour {
    public Vector3 Axis;
    public Vector3 StartOffset;

    public Vector3 minAngle;
    public Vector3 maxAngle;

    private void OnEnable () {
        StartOffset = transform.localPosition;
    }
}