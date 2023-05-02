using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	private ShipMovement shipMovement;

	private void Start() {
		shipMovement = GetComponent<ShipMovement>();
	}

	private void Update() {
		shipMovement.Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown(1)) {
			shipMovement.enabled = !shipMovement.enabled;
			if (!shipMovement.enabled) {
				GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			}
		}
	}
}
