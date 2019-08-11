using System.Collections;
using UnityEngine;

public class Biped : MonoBehaviour {
    public Transform leftFoot, rightFoot;
    public float verticalOffset;
    public Vector3 velocity;
    public float smoothTime = 1f;

    public StepFinder leftFinder, rightFinder;
    public float stepDelay = 5;
    public bool whichFoot;
    public IKControl leftControl, rightControl;
    public float tweenTime = 1;

    private IEnumerator Start () {
        while (true) {
            yield return new WaitForSeconds (stepDelay);
            whichFoot = !whichFoot;
            if (whichFoot) {
                yield return TweenLegTarget (leftControl.target.transform, leftFinder.FindStep ());
            } else {
                yield return TweenLegTarget (rightControl.target.transform, rightFinder.FindStep ());
            }
        }
    }

    [ContextMenu ("Move Leg")]
    public void MoveLeg () {
        whichFoot = !whichFoot;
        if (whichFoot) {
            StartCoroutine (TweenLegTarget (leftControl.target.transform, leftFinder.FindStep ()));
        } else {
            StartCoroutine (TweenLegTarget (rightControl.target.transform, rightFinder.FindStep ()));
        }
    }

    private IEnumerator TweenLegTarget (Transform ikTarget, Vector3 target) {
        float t = 0;
        Vector3 start = ikTarget.position;
        while (t < tweenTime) {
            t += Time.deltaTime;
            ikTarget.position = Vector3.Lerp (start, target, t);
            yield return null;
        }
        // ikTarget.position = target;
        // yield return null;
    }

    private void LateUpdate () {
        Vector3 feetAvg = leftFoot.position + rightFoot.position;
        feetAvg *= 0.5f;
        // Debug.Log (leftFoot.position + " " + rightFoot.position + " " + feetAvg);
        feetAvg.y += verticalOffset;
        // feetAvg.y = Mathf.Min (leftFoot.position.y, rightFoot.position.y) + verticalOffset;
        // feetAvg.y = transform.position.y;

        // transform.position = feetAvg;
        // transform.position = Vector3.Lerp (transform.position, feetAvg, 0.5f);
        transform.position = Vector3.SmoothDamp (transform.position, feetAvg, ref velocity, smoothTime);
    }
}