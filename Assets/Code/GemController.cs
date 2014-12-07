using UnityEngine;
using System.Collections;

public class GemController : MonoBehaviour {

	public ScenarioController scenario;
	public Transform model;

	void OnTriggerEnter(Collider other) {
		var playerController = other.GetComponent<PlayerController>();
		if (playerController != null) {
			scenario.Advance();
			gameObject.SetActive(false);
		}
	}

	void Update() {
		model.localRotation = Quaternion.Euler(new Vector3(270, Time.time * 30, 0));
		model.localPosition = Vector3.up * (0.2f + 0.1f * Mathf.Sin(Time.time * 2));
	}
}
