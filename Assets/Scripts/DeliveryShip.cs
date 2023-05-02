using System.Collections.Generic;
using UnityEngine;

public class DeliveryShip : MonoBehaviour {
	[SerializeField]
	private List<GameObject> indicators;

	private List<Package> packages = new();

	private void Start() {
		foreach (GameObject indicator in indicators) { 
			indicator.SetActive(false);
		}
	}

	public bool AddPackage(Package package) {
		if (packages.Count < indicators.Count) {
			packages.Add(package);
			return true;
		}

		return false;
	}

	public void RemovePackage(Package package) {
		if (!packages.Contains(package))
			return;

		packages.Remove(package);
	}

	private void Update() {
		for (int i = 0; i < indicators.Count; i++) {
			GameObject indicator = indicators[i];

			if (packages.Count > i) {
				indicator.SetActive(true);

				Package package = packages[i];

				Vector2 target = package.Dropoff.transform.position;

				Vector2 dir = (target - (Vector2) transform.position).normalized;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

				Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
				indicator.transform.rotation = Quaternion.Lerp(indicator.transform.rotation, targetRotation, 10 * Time.deltaTime);
			} else {
				indicator.SetActive(false);
			}

		}
	}
}
