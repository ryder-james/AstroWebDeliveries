using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageFinder : MonoBehaviour {
	[SerializeField]
	private Transform rotationTarget;

	public Planet Planet { get; set; }
	public Package Package { get; set; }
	public Collider2D Bounds { get; set; }

	private void Start() {
		Planet.PackagePickedUp += OnPackagePickedUp;
	}

	private void Update() {
		transform.position = Bounds.ClosestPoint(Planet.transform.position);
	}

	private void OnPackagePickedUp(Package pkg) {
		if (Package == pkg) {
			Planet.PackagePickedUp -= OnPackagePickedUp;
			Destroy(gameObject);
		}
	}
}
