using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipMovement : MonoBehaviour {
	[SerializeField]
	private float maxSpeed = 10f;

	[SerializeField]
	private float speed = 0.1f;

	[SerializeField]
	private Transform targetObject;

	[SerializeField]
	private BoxCollider2D bounds;

	private Rigidbody2D rb;

	private float? minX, minY, maxX, maxY;
	private float MinX => minX ??= bounds.bounds.min.x;
	private float MaxX => maxX ??= bounds.bounds.max.x;
	private float MinY => minY ??= bounds.bounds.min.y;
	private float MaxY => maxY ??= bounds.bounds.max.y;

	private Vector2 target;
	public Vector2 Target { 
		get => target; 
		set {
			Vector2 tempTarget = value;
			if (bounds != null) {
				tempTarget.x = Mathf.Clamp(tempTarget.x, MinX, MaxX);
				tempTarget.y = Mathf.Clamp(tempTarget.y, MinY, MaxY);
			}
			target = tempTarget;
		}
	}

	public Transform TargetObject { get => targetObject; set => targetObject = value; }
	public float Speed { get => speed; set => speed = value; }
	public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		if (TargetObject != null)
			Target = TargetObject.position;
	}

	private void FixedUpdate() {
		rb.AddForce(Speed * Vector2.Distance(transform.position, Target) * (Target - (Vector2) transform.position).normalized, ForceMode2D.Impulse);
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected() {
		if (!rb)
			return;

		Gizmos.DrawSphere(Target, 0.1f);
	}
#endif
}
