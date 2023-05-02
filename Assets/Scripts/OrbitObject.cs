using UnityEngine;

public class OrbitObject : MonoBehaviour {
	public Transform Target { get; set; }
	public float OrbitSpeed { get; set; }
	public float OrbitDistance { get; set; }

	private float orbitAngle;

	private void Start() {
		GetComponent<Rigidbody2D>().isKinematic = true;

		// Calculate the initial angle between the target and spawned object
		Vector2 directionToTarget = Target.position - transform.position;
		orbitAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x);
	}

	private void Update() {
		if (Target != null) {
			// Update the orbit angle based on the orbit speed
			orbitAngle += OrbitSpeed * Time.deltaTime;

			// Calculate the new position of the spawned object
			Vector2 newPosition = new Vector2(
				Target.position.x + OrbitDistance * Mathf.Cos(orbitAngle),
				Target.position.y + OrbitDistance * Mathf.Sin(orbitAngle)
			);

			// Set the new position of the spawned object
			transform.position = newPosition;
		}
	}

	private void OnDestroy() {
		GetComponent<Rigidbody2D>().isKinematic = false;
	}
}