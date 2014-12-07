using UnityEngine;
using System.Collections;

public class HeavyButtonController : MonoBehaviour {

	public ScenarioController scenario;

	public float minimumVelocity = 38.0f;

	void OnTriggerEnter(Collider other) {
		var playerController = other.GetComponent<PlayerController>();
		if (playerController != null) {
			if (playerController.lastImpactVelocity < -minimumVelocity || playerController.velocity.y < -minimumVelocity) {
				scenario.Advance();
				gameObject.SetActive(false);
			}
		}
	}
}
