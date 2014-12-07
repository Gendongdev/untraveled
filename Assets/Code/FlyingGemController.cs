using UnityEngine;
using System.Collections;

public class FlyingGemController : MonoBehaviour {

	public ScenarioController scenario;
	public Transform model;
	
	void Update() {
		model.localRotation = Quaternion.Euler(new Vector3(270, Time.time * 30, 0));
		model.localPosition = Vector3.up * (0.2f + 0.1f * Mathf.Sin(Time.time * 2));
	}

	void OnTriggerEnter(Collider other) {
		var playerController = other.GetComponent<PlayerController>();
		if (playerController != null) {
			playerController.PlayPickup();
			scenario.Advance();
			gameObject.SetActive(false);
		}
	}
}
