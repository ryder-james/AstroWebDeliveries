using System.Collections;
using UnityEngine;

public class GrowOnStart : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 startScale = Vector3.one * 0.01f;

    [SerializeField]
    private Vector3 endScale = Vector3.one;

    [SerializeField]
    private float secondsToFullScale = 1;

    [SerializeField]
    private AnimationCurve growthCurve;

	private void Start() {
        StartCoroutine(Grow());
	}

	private IEnumerator Grow(bool destroy = true) {
        Vector3 start = startScale;
        Vector3 end = endScale;

        target.localScale = startScale;
        for (float t = 0; t < secondsToFullScale; t += Time.deltaTime) { 
            target.localScale = Vector3.Lerp(start, end, growthCurve.Evaluate(t / secondsToFullScale));
            yield return new WaitForEndOfFrame();
        }
        target.localScale = endScale;

        if (destroy)
            Destroy(this);
    }

}
