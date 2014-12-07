using UnityEngine;
using System.Collections;

public class HeavyButtonController : MonoBehaviour {

	public ScenarioController scenario;
	public Transform model;

	public float minimumVelocity = 38.0f;

	void OnEnable() {
		model.localPosition = new Vector3(0, -0.5f, 0);
	}

	void Update() {
		model.localPosition = new Vector3(0, Mathf.Min(model.localPosition.y + 4 * Time.deltaTime, 0.25f), 0);
	}

	void OnTriggerEnter(Collider other) {
		var playerController = other.GetComponent<PlayerController>();
		if (playerController != null) {
			if (playerController.lastImpactVelocity < -minimumVelocity || playerController.velocity.y < -minimumVelocity) {
				playerController.PlayButtonSuccess();
				scenario.Advance();
				gameObject.SetActive(false);
			} else {
				playerController.PlayButtonFail();
			}
		}
	}
}
