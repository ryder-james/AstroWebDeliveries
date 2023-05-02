using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
	#region Constants
	private const float MinRadius = 0.25f;
	private const float MaxRadius = 2.5f;
	private const float TrueMinDropoffRadius = 2f;
	private const float TrueMaxDropoffRadius = 8f;
	private const float MinRotationSpeed = -15f;
	private const float MaxRotationSpeed = 15f;
	private const int PackageLimit = 1;
	#endregion

	#region Serialized Properties
	[Header("Gameplay")]
	[SerializeField, Range(MinRadius, MaxRadius)]
	private float radius = 1;

	[SerializeField, Min(TrueMinDropoffRadius)]
	private float packageDropoffRadius = 3;

	[SerializeField, Range(1, 3)]
	private float indicatorDistance = 1.1f;

	[SerializeField]
	private string locationName;

	[SerializeField]
	private GameObject indicator, packagePrefab;

	[SerializeField]
	private Vector2 packageOrbitSpeedRange = new Vector2(-2, 2);

	[SerializeField]
	private CircleCollider2D collision, packageDropoff;

	[Header("Visuals")]
	[SerializeField, Range(MinRotationSpeed, MaxRotationSpeed)]
	private float rotationSpeed = 1;

	[SerializeField]
	private Transform visualsTransform;

	[SerializeField]
	private SpriteRenderer waterSprite, landSprite;

	[SerializeField]
	private SpriteList waterSprites, landSprites;
	#endregion

	#region Members
	private bool active;
	#endregion

	#region Properties
	private Color RandomColor => new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

	public bool Active { get => active; set => indicator.SetActive(active = value); }
	public string LocationName => locationName;

	private List<Package> packages = new();

	public System.Action<Package> PackagePickedUp { get; set; }
	#endregion

	#region Mono
	private void Start() {
		Active = false;
	}

	private void Update() {
		if (visualsTransform) {
			Vector3 rot = visualsTransform.rotation.eulerAngles;
			rot.z += rotationSpeed * Time.deltaTime;
			rot.z %= 360;
			if (rot.z < -360) {
				rot.z = 0;
			}
			visualsTransform.rotation = Quaternion.Euler(rot);
		}
	}

#if UNITY_EDITOR
	private void OnValidate() {
		UpdateSettings();
	}
#endif
	#endregion

	[ContextMenu("Respawn Package")]
	public void RespawnPackage() {
		DestroyPackage();
		SpawnPackage(out Package _);
	}

	[ContextMenu("Spawn Package")]
	public bool SpawnPackage(out Package pkg) {
		pkg = null;

		if (packages.Count >= PackageLimit)
			return false;

		Vector2 spawnDirection = Random.insideUnitCircle.normalized;
		Vector2 spawnPosition = (Vector2) transform.position + spawnDirection * packageDropoffRadius;

		// Instantiate prefab
		GameObject spawnedObject = Instantiate(packagePrefab, spawnPosition, Quaternion.identity);

		// Ensure the spawned object has a Rigidbody2D component
		if (!spawnedObject.TryGetComponent(out Rigidbody2D rb)) {
			rb = spawnedObject.AddComponent<Rigidbody2D>();
		}

		if (!spawnedObject.TryGetComponent(out pkg)) {
			pkg = spawnedObject.AddComponent<Package>();
		}
		pkg.OrbitedPlanet = this;
		pkg.SetRandomDropoff();
		packages.Add(pkg);

		OrbitObject orbitObject = spawnedObject.AddComponent<OrbitObject>();
		orbitObject.Target = transform;
		orbitObject.OrbitSpeed = Random.Range(packageOrbitSpeedRange.x, packageOrbitSpeedRange.y);
		orbitObject.OrbitDistance = packageDropoffRadius;

		return true;
	}

	[ContextMenu("Destroy Orbiting Package")]
	public void DestroyPackage() {
		if (packages.Count == 0)
			return;

		Package pkg = packages[packages.Count - 1];
		packages.Remove(pkg);
		Destroy(pkg.gameObject);
	}

	public void RemovePackage(Package pkg) {
		if (packages.Count == 0 || !packages.Contains(pkg))
			return;

		packages.Remove(pkg);
		pkg.OrbitedPlanet = null;
		Destroy(pkg.GetComponent<OrbitObject>());
		PackagePickedUp?.Invoke(pkg);
	}

	#region Appearance Stuff
	public float MinDropoffRadius => Mathf.Max(radius * 2, TrueMinDropoffRadius);
	public float MaxDropoffRadius => Mathf.Min(radius * 10, TrueMaxDropoffRadius);
	[ContextMenu("Randomize")]
	public void Randomize() {
		radius = Random.Range(MinRadius, MaxRadius);
		packageDropoffRadius = radius * Random.Range(MinDropoffRadius, MaxDropoffRadius);
		rotationSpeed = Random.Range(MinRotationSpeed, MaxRotationSpeed);
		bool cwOrbit = Random.Range(0, 2) == 0;
		packageOrbitSpeedRange.x = cwOrbit ? 0.5f : -2;
		packageOrbitSpeedRange.y = cwOrbit ? 2 : -0.5f;

		if (waterSprites != null && waterSprites.Count > 0 && landSprites != null && landSprites.Count == waterSprites.Count) {
			int index = Random.Range(0, waterSprites.Count);
			waterSprite.sprite = waterSprites[index];
			landSprite.sprite = landSprites[index];
		}
		if (waterSprite) {
			waterSprite.color = RandomColor;
		}
		if (landSprite) {
			landSprite.color = RandomColor;
		}
		if (visualsTransform) {
			Vector3 rot = visualsTransform.rotation.eulerAngles;
			rot.z = Random.Range(0f, 360f);
			visualsTransform.rotation = Quaternion.Euler(rot);
		}

		GeneratePlanetName();
		UpdateSettings();
	}

	public void UpdateSettings() {
		packageDropoffRadius = Mathf.Clamp(packageDropoffRadius, MinDropoffRadius, MaxDropoffRadius);

		if (visualsTransform) {
			visualsTransform.localScale = 2f * radius * Vector2.one;
		}

		if (collision) {
			collision.radius = radius;
		}

		if (packageDropoff) {
			packageDropoff.radius = packageDropoffRadius;
		}

		if (indicator) {
			Vector2 pos = indicator.transform.localPosition;
			pos.y = radius * indicatorDistance;
			indicator.transform.localPosition = pos;
		}

		if (!string.IsNullOrEmpty(locationName)) {
			gameObject.name = locationName;
		}
	}

	[ContextMenu("Generate Name")]
	public void GeneratePlanetName() {
		locationName = planetPrefixes[Random.Range(0, planetPrefixes.Length)] + planetPostfixes[Random.Range(0, planetPostfixes.Length)];
		UpdateSettings();
	}

	private static string[] planetPrefixes = {
		"Astra",
		"Celesto",
		"Thalassa",
		"Terra",
		"Cosmo",
		"Exo",
		"Helio",
		"Luna",
		"Nova",
		"Orbi",
		"Pando",
		"Quasar",
		"Stellar",
		"Vortex",
		"Xeno",
		"Zeta",
		"Galacto",
		"Nebulo",
		"Pyro",
		"Chrono"
	};

	private static string[] planetPostfixes = {
		"taris",
		"theon",
		"nix",
		"dra",
		"gaius",
		"crysta",
		"pia",
		"vora",
		"thos",
		"ron",
		"xar",
		"lem",
		"zor",
		"phos",
		"ium",
		"pora",
		"cyon",
		"tis",
		"karn",
		"mara"
	};
	#endregion

	public override bool Equals(object other) => (other is Planet otherPlanet) && locationName == otherPlanet.locationName;
	public override int GetHashCode() => System.HashCode.Combine(base.GetHashCode(), locationName);
}
