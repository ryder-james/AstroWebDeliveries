using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class GravityEmitter : MonoBehaviour {
	[SerializeField, Range(0.5f, 10f)] private float gravityMultiplier = 1;
	[SerializeField, Range(0.5f, 10f)] private float gravityRadiusMultiplier = 1;
	[SerializeField] private AnimationCurve gravityStrengthCurve;

	private Rigidbody2D rb;
	private Rigidbody2D Rigidbody => rb != null ? rb : rb = GetComponent<Rigidbody2D>();

	private CircleCollider2D coll;
	private CircleCollider2D Collider => coll != null ? coll : coll = GetComponents<CircleCollider2D>().First(c => !c.isTrigger);

	private float Radius => Collider.radius * gravityRadiusMultiplier * 2;
	private float Gravity => Rigidbody.mass * gravityMultiplier;

	private void FixedUpdate() {
		foreach (Collider2D other in Physics2D.OverlapCircleAll(transform.position, Radius)) {
			if (other.gameObject == gameObject || !other.TryGetComponent(out Rigidbody2D rigidbody))
				continue;

			Vector2 dir = (transform.position - other.transform.position).normalized;
			float dist = Vector2.Distance(transform.position, other.transform.position);

			dir *= Gravity;
			//dir /= dist;
			dir *= gravityStrengthCurve.Evaluate(Mathf.InverseLerp(0, Radius, dist));
			rigidbody.AddForce(dir);
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected() {
		if (!isActiveAndEnabled)
			return;

		CircleCollider2D collider = GetComponent<CircleCollider2D>();
		Handles.DrawWireDisc(transform.position, Vector3.back, collider.radius * 2 * gravityRadiusMultiplier);
	}
#endif
}
