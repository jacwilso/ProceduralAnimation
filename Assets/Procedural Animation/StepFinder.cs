using UnityEngine;

public class StepFinder : MonoBehaviour {
    public Ray ray;
    public RaycastHit hitInfo;
    public float legLength;
    public Transform foot;

    public Vector2 rayArc;
    public int raysInArc;
    public LayerMask raycastLayer;

    public int selectedRay = -1;

    private void Start () {
        legLength = Mathf.Abs (transform.position.y - foot.position.y);
    }

    public Vector3 FindStep () {
        float x, y, angle = rayArc.y * Mathf.Deg2Rad;
        float radPerRay = (rayArc.y - rayArc.x) / (raysInArc - 1) * Mathf.Deg2Rad;
        Ray ray = new Ray (transform.position, Vector3.forward);
        Vector3 direction = new Vector3 ();
        selectedRay = -1;
        for (int i = 0; i < raysInArc; i++) {
            x = Mathf.Cos (angle);
            y = Mathf.Sin (angle);
            angle -= radPerRay;
            ray.origin = transform.position;
            direction.y = y;
            direction.z = x;
            ray.direction = direction;
            Debug.DrawRay (ray.origin, ray.direction, Color.red);
            if (Physics.Raycast (ray, out hitInfo, legLength, raycastLayer, QueryTriggerInteraction.Ignore)) {
                selectedRay = i;
                // Debug.Log ("HIT " + i + " " + hitInfo.point);
                return hitInfo.point;
            }
        }
        // Debug.Log ("MISS");
        return foot.position;
    }
}