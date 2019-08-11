using UnityEngine;

public class IKControl : MonoBehaviour {

    public Transform target;
    public JointIK[] Joints;
    public Transform EndEffector;
    public float SamplingDistance, LearningRate, DistanceThreshold;

    public Vector3[] points;
    public Color[] colors;
    public Gradient colorGradient;
    public bool debug;

    private void Start () {
        points = new Vector3[Joints.Length + 1];
        colors = new Color[Joints.Length + 1];
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = colorGradient.Evaluate ((float) i / colors.Length);
        }
    }

    private void Update () {
        InverseKinematics (target.position);
    }

    public void InverseKinematics (Vector3 target) {
        Quaternion[] angles = new Quaternion[Joints.Length];
        for (int i = 0; i < angles.Length; i++) {
            angles[i] = Joints[i].transform.localRotation;
        }

        InverseKinematics (target, angles);

        for (int i = 0; i < angles.Length; i++) {
            Joints[i].transform.localRotation = angles[i];
        }
    }

    public void InverseKinematics (Vector3 target, Quaternion[] angles) {
        if (DistanceFromTarget (target, angles) < DistanceThreshold)
            return;

        for (int i = Joints.Length - 1; i >= 0; i--) {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradient (target, angles, i);
            angles[i] *= Quaternion.AngleAxis (-LearningRate * gradient, Joints[i].Axis);

            // constraint
            float angle;
            Vector3 axis;
            angles[i].ToAngleAxis (out angle, out axis);
            axis = (angle * axis).Clamp (Joints[i].minAngle, Joints[i].maxAngle);
            angles[i] = Quaternion.Euler (axis);

            // Early termination
            if (DistanceFromTarget (target, angles) < DistanceThreshold)
                return;
        }
    }

    public float PartialGradient (Vector3 target, Quaternion[] angles, int i) {
        // Saves the angle,
        // it will be restored later
        Quaternion angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget (target, angles);

        angles[i] *= Quaternion.AngleAxis (SamplingDistance, Joints[i].Axis);
        float f_x_plus_d = DistanceFromTarget (target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;

        return gradient;
    }

    public float DistanceFromTarget (Vector3 target, Quaternion[] angles) {
        Vector3 point = ForwardKinematics (angles);
        Debug.DrawLine (point, target);
        return Vector3.Distance (point, target);
    }

    public Vector3 ForwardKinematics (Quaternion[] angles) {
        return ForwardKinematics (angles, Joints, EndEffector);
    }

    public Vector3 ForwardKinematics (Quaternion[] angles, JointIK[] joints, Transform endEffector) { // TODO make more than 1D
        Vector3 prevPoint = joints[0].transform.position;
        Quaternion rotation = angles[0];
        points[0] = prevPoint;
        for (int i = 1; i < joints.Length; i++) {
            prevPoint += rotation * joints[i].StartOffset;
            points[i] = prevPoint;
            rotation *= angles[i];
        }
        prevPoint += rotation * endEffector.localPosition;
        points[joints.Length] = prevPoint;

        return prevPoint;
    }

    private void OnDrawGizmos () {
        if (debug) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere (target.position, 1f);

            if (points.Length == 0) {
                return;
            }
            for (int i = 0; i < points.Length; i++) {
                Gizmos.color = colorGradient.Evaluate ((float) i / points.Length);
                Gizmos.DrawSphere (points[i], 0.4f);
            }
        }
    }
}