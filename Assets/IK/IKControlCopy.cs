using UnityEngine;

public class IKControlCopy : MonoBehaviour {

    public Transform target;
    public JointIK[] Joints;
    public float SamplingDistance, LearningRate, DistanceThreshold;

    public Vector3[] points = new Vector3[3];
    Color[] colors = new Color[] { Color.red, Color.blue, Color.green };

    private void Update () {
        InverseKinematics (target.position);
    }

    public void InverseKinematics (Vector3 target) {
        float[] angles = new float[Joints.Length];
        for (int i = 0; i < angles.Length; i++) {
            angles[i] = Vector3.Dot (Joints[i].Axis, Joints[i].transform.localEulerAngles);
        }
        InverseKinematics (target, angles);
        for (int i = 0; i < angles.Length; i++) {
            Joints[i].transform.eulerAngles = angles[i] * Joints[i].Axis;
        }
    }

    // public void InverseKinematics (Vector3 target, float[] angles) {
    //     for (int i = 0; i < Joints.Length; i++) {
    //         // Gradient descent
    //         // Update : Solution -= LearningRate * Gradient
    //         float gradient = PartialGradient (target, angles, i);
    //         angles[i] -= LearningRate * gradient;
    //     }
    // }

    public void InverseKinematics (Vector3 target, float[] angles) {
        if (DistanceFromTarget (target, angles) < DistanceThreshold)
            return;

        for (int i = Joints.Length - 1; i >= 0; i--) {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradient (target, angles, i);
            angles[i] -= LearningRate * gradient;

            // Early termination
            if (DistanceFromTarget (target, angles) < DistanceThreshold)
                return;
        }
    }

    public float PartialGradient (Vector3 target, float[] angles, int i) {
        // Saves the angle,
        // it will be restored later
        float angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget (target, angles);

        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget (target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;

        return gradient;
    }

    public float DistanceFromTarget (Vector3 target, float[] angles) {
        Vector3 point = ForwardKinematics (angles);
        Debug.DrawLine (point, target);
        return Vector3.Distance (point, target);
    }

    public Vector3 ForwardKinematics (float[] angles) {
        return ForwardKinematics (angles, Joints);
    }

    public Vector3 ForwardKinematics (float[] angles, JointIK[] joints) { // TODO make more than 1D
        Vector3 prevPoint = joints[0].transform.position;
        points[0] = prevPoint;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < joints.Length; i++) {
            rotation *= Quaternion.AngleAxis (angles[i - 1], joints[i - 1].Axis);
            Vector3 nextPoint = prevPoint + rotation * joints[i].StartOffset;

            prevPoint = nextPoint;
            points[i] = prevPoint;
        }

        return prevPoint;
    }

    private void OnDrawGizmos () {
        for (int i = 0; i < points.Length; i++) {
            Gizmos.color = colors[i];
            Gizmos.DrawSphere (points[i], 1);
        }
    }
}