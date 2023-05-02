using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToVelocity : MonoBehaviour {
	[SerializeField]
	private Rigidbody2D source;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private float lookSpeed = 3f;

	private void Update() {
		if (!source || !target) return;

		Vector2 dir = source.velocity.normalized;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

		Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		target.rotation = Quaternion.Lerp(target.rotation, targetRotation, lookSpeed * Time.deltaTime);
	}
}
