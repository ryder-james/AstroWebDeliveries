using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour {
	public Planet Dropoff { get; private set; }
	public Planet OrbitedPlanet { get; set; }

	private DeliveryShip Ship { get; set; }

	public System.Action DroppedOff { get; set; }

	private void Start() {
		SetRandomDropoff();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (Ship != null && collision.gameObject.TryGetComponent(out Planet hitLocation) && Dropoff.Equals(hitLocation)) { 
			Dropoff.Active = false;
			Ship.RemovePackage(this);
			Ship = null;
			DroppedOff?.Invoke();
			Destroy(gameObject);
		} else if (OrbitedPlanet && collision.gameObject.TryGetComponent(out DeliveryShip ship) && ship.AddPackage(this)) {
			var mvt = gameObject.AddComponent<ShipMovement>();
			mvt.TargetObject = ship.transform;
			mvt.MaxSpeed = ship.GetComponent<ShipMovement>().MaxSpeed;
			mvt.Speed = 0.5f;
			OrbitedPlanet.RemovePackage(this);
			Dropoff.Active = true;
			Ship = ship;
		}
	}

	public void SetRandomDropoff() => SetDropoff(GameManager.Instance.GetRandomPlanet());

	public void SetDropoff(Planet planet) {
		Dropoff = planet;
	}
}
