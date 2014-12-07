using UnityEngine;
using System.Collections;

public class FallingGemController : MonoBehaviour {

	public ScenarioController scenario;
	public ScreenController screen;
	public Transform model;

	public float gravity = 40.0f;
	public float terminalVelocity = 20.0f;

	[System.NonSerialized]
	public Vector2 screenSize;

	[System.NonSerialized]
	float velocity;
	[System.NonSerialized]
	bool landed;

	void Start() {
		screenSize = screen.size;
		velocity = 0;
		landed = false;
	}

	void FixedUpdate() {
		var wrappingOffset = Vector2.zero;

		if (!landed) {
			velocity = Mathf.Min(terminalVelocity, velocity + gravity * Time.fixedDeltaTime);
			wrappingOffset.y = -velocity * Time.fixedDeltaTime;
			RaycastHit hitInfo;
			// TODO: fetch this from sphere collider
			if (Physics.SphereCast(transform.position, 0.3f, Vector3.down, out hitInfo, -wrappingOffset.y)) {
				wrappingOffset.y = -hitInfo.distance;
				velocity = 0;
				landed = true;
			}
		}

		// TODO: deduplicate with code in LevelController
		if (transform.localPosition.x >= screenSize.x * 0.5f)
			wrappingOffset.x = -screenSize.x;
		if (transform.localPosition.y >= screenSize.y * 0.5f)
			wrappingOffset.y = -screenSize.y;
		if (transform.localPosition.x <= -screenSize.x * 0.5f)
			wrappingOffset.x = screenSize.x;
		if (transform.localPosition.y <= -screenSize.y * 0.5f)
			wrappingOffset.y = screenSize.y;
		transform.localPosition += (Vector3)wrappingOffset;
	}

	void Update() {
		model.localRotation = Quaternion.Euler(new Vector3(270, Time.time * 30, 0));
		model.localPosition = Vector3.up * (0.2f + 0.1f * Mathf.Sin(Time.time * 2));
	}

	void OnTriggerEnter(Collider other) {
		var playerController = other.GetComponent<PlayerController>();
		if (playerController != null) {
			scenario.Advance();
			gameObject.SetActive(false);
		}
	}
}
