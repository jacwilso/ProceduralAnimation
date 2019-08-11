using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (StepFinder))]
public class StepFinderEditor : Editor {
    private StepFinder targetFinder;

    public override void OnInspectorGUI () {
        base.OnInspectorGUI ();
    }

    private void OnEnable () {
        targetFinder = target as StepFinder;
    }

    private void OnSceneGUI () {
        if (targetFinder.rayArc.x > targetFinder.rayArc.y) {
            return;
        }

        Handles.color = Color.white;
        float angle = targetFinder.rayArc.x * Mathf.Deg2Rad;
        float x = Mathf.Cos (angle);
        float y = Mathf.Sin (angle);
        Handles.DrawWireArc (targetFinder.transform.position, targetFinder.transform.right, new Vector3 (0, y, x), -Mathf.Abs (targetFinder.rayArc.x - targetFinder.rayArc.y), targetFinder.legLength);

        Vector3[] lines = new Vector3[targetFinder.raysInArc * 2];
        float radPerRay = (targetFinder.rayArc.y - targetFinder.rayArc.x) / (targetFinder.raysInArc - 1) * Mathf.Deg2Rad;
        for (int i = 0; i < targetFinder.raysInArc; i++) {
            lines[2 * i] = targetFinder.transform.position;
            x = Mathf.Cos (angle);
            y = Mathf.Sin (angle);
            lines[2 * i + 1] = lines[2 * i] + targetFinder.legLength * new Vector3 (0, y, x);
            angle += radPerRay;
        }
        Handles.DrawLines (lines);

        if (targetFinder.selectedRay >= 0) {
            Handles.color = Color.green;
            Handles.DrawLine (lines[2 * targetFinder.selectedRay], lines[2 * targetFinder.selectedRay + 1]);
        }
    }
}