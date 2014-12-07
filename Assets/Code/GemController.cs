using UnityEngine;
using System.Collections;

public class GemController : MonoBehaviour {

	public ScenarioController scenario;
	public Transform model;

	[System.NonSerialized]
	Vector3 baseScale;

	void Start() {
		baseScale = model.transform.localScale;
		model.transform.localScale = Vector3.zero;
	}

	void OnTriggerEnter(Collider other) {
		var playerController = other.GetComponent<PlayerController>();
		if (playerController != null) {
			playerController.PlayPickup();
			scenario.Advance();
			gameObject.SetActive(false);
		}
	}

	void Update() {
		model.localRotation = Quaternion.Euler(new Vector3(270, Time.time * 30, 0));
		model.localPosition = Vector3.up * (0.2f + 0.1f * Mathf.Sin(Time.time * 2));
		model.transform.localScale = Vector3.Lerp(model.transform.localScale, baseScale, 10.0f * Time.deltaTime);
	}
}
