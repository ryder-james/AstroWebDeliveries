using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DupeIdentifier : MonoBehaviour {
	[ContextMenu("Check")]
	public void CheckForDupes() {
		List<string> names = new List<string>();
		int dupes = 0;
		for (int i = 0; i < transform.childCount; i++) {
			string name = transform.GetChild(i).name;
			if (names.Contains(name)) {
				Debug.LogWarning($"Found identical name {name} at {i}!");
				transform.GetChild(i).name = $"! - {name}";
				dupes++;
			} else {
				names.Add(name);
			}
		}
		Debug.Log($"Found {dupes} duplicates in {transform.childCount} names.");
	}
}
