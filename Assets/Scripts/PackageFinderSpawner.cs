using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageFinderSpawner : MonoBehaviour {
	[SerializeField]
	private Collider2D indicatorEdge;

	[SerializeField]
	private GameObject indicatorPrefab;

	private void Start() {
		GameManager.Instance.PackageSpawned += PackageSpawned;
	}

	private void OnDestroy() {
		GameManager.Instance.PackageSpawned -= PackageSpawned;
	}

	private void PackageSpawned(Planet p, Package pkg) {
		if (!indicatorEdge)
			return;

		GameObject newIndicator = Instantiate(indicatorPrefab, indicatorEdge.ClosestPoint(p.transform.position), Quaternion.identity, transform);
		PackageFinder finder = newIndicator.GetComponent<PackageFinder>();

		finder.Planet = p;
		finder.Package = pkg;
		finder.Bounds = indicatorEdge;
	}
}
