using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour {
#if UNITY_EDITOR
	[SerializeField] private GameObject prefab;
	[SerializeField] private Transform objectParent;
	[SerializeField] private int numberOfCopies = 10;
	[SerializeField] private BoxCollider2D spawnArea, exclusionArea;
	[SerializeField] private float minDistanceBetweenObjects = 2.5f;
	[SerializeField] private int maxAttemptsPerSpawn = 10;

	[ContextMenu("Spawn")]
	public void DoSpawn() {
		for (int i = objectParent.childCount - 1; i >= 0; i--) {
			DestroyImmediate(objectParent.GetChild(i).gameObject);
		}

		SpawnPrefabs(numberOfCopies);
	}

	private void SpawnPrefabs(int count) {
		List<Vector2> spawnedPositions = new List<Vector2>();
		int successCount = 0;
		for (int i = 0; i < count; i++) {
			int attempts = 0;
			Vector2 randomPosition = GetRandomPositionInBox(spawnArea.bounds);

			while (attempts < maxAttemptsPerSpawn && (IsPositionInExclusionArea(randomPosition) || IsPositionTooCloseToOthers(ref spawnedPositions, randomPosition))) {
				randomPosition = GetRandomPositionInBox(spawnArea.bounds);
				attempts++;
			}

			if (attempts < maxAttemptsPerSpawn) {
				GameObject newPlanetGO = PrefabUtility.InstantiatePrefab(prefab).GameObject();
				newPlanetGO.transform.position = randomPosition;
				newPlanetGO.transform.parent = objectParent;

				Planet planet = newPlanetGO.GetComponent<Planet>();
				planet.Randomize();
				spawnedPositions.Add(randomPosition);
				successCount++;
			} else {
				Debug.LogWarning("Could not find a suitable position for a prefab. Skipping...");
			}
		}
		Debug.Log($"Succeeded on generating {successCount} out of {count}.");
	}

	private Vector2 GetRandomPositionInBox(Bounds bounds) {
		float x = Random.Range(bounds.min.x, bounds.max.x);
		float y = Random.Range(bounds.min.y, bounds.max.y);

		return new Vector2(x, y);
	}

	private bool IsPositionTooCloseToOthers(ref List<Vector2> spawnedPositions, Vector2 position) {
		foreach (Vector2 existingPosition in spawnedPositions) {
			if (Vector2.Distance(position, existingPosition) < minDistanceBetweenObjects) {
				return true;
			}
		}
		return false;
	}

	private bool IsPositionInExclusionArea(Vector2 position) {
		if (exclusionArea != null) {
			return exclusionArea.bounds.Contains(position);
		}
		return false;
	}

	//private void SpawnPrefabs(int count) {
	//	for (int i = 0; i < count; i++) {
	//		Vector2 randomPosition = GetRandomPositionInBox(spawnArea.bounds);
	//		GameObject newPlanetGO = Instantiate(prefab, randomPosition, Quaternion.identity, objectParent);
	//		Planet planet = newPlanetGO.GetComponent<Planet>();
	//		planet.Randomize();
	//	}
	//}
#endif
}
