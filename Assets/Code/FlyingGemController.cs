using UnityEngine;
using System.Collections;

public class FlyingGemController : MonoBehaviour {

	public ScenarioController scenario;

	void OnTriggerEnter(Collider other) {
		var playerController = other.GetComponent<PlayerController>();
		if (playerController != null) {
			scenario.Advance();
			gameObject.SetActive(false);
		}
	}
}
