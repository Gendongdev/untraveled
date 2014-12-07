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
	[System.NonSerialized]
	Vector3 baseScale;
	[System.NonSerialized]
	Material material;
	[System.NonSerialized]
	bool captured;

	void Start() {
		screenSize = screen.size;
		velocity = 0;
		landed = false;
		baseScale = model.transform.localScale;
		model.transform.localScale = Vector3.zero;
		material = model.GetComponent<MeshRenderer>().material;
		captured = false;
	}

	void OnEnable() {
		material = model.GetComponent<MeshRenderer>().material;
	}

	void FixedUpdate() {
		var wrappingOffset = Vector2.zero;

		if (!landed) {
			velocity = Mathf.Min(terminalVelocity, velocity + gravity * Time.fixedDeltaTime);
			wrappingOffset.y = -velocity * Time.fixedDeltaTime;
			RaycastHit hitInfo;
			// TODO: fetch this from sphere collider
			var hasHit = Physics.SphereCast(transform.position + Vector3.up, 0.3f, Vector3.down, out hitInfo, -wrappingOffset.y + 1);
			if (hasHit) {
				wrappingOffset.y = -(hitInfo.distance - 1);
				velocity = 0;
				landed = true;
			}
		}

		// TODO: deduplicate with code in LevelController
		if (transform.localPosition.x >= screenSize.x * 0.5f)
			wrappingOffset.x = -screenSize.x;
		if (transform.localPosition.y >= screenSize.y * 0.5f)
			wrappingOffset.y += -screenSize.y;
		if (transform.localPosition.x <= -screenSize.x * 0.5f)
			wrappingOffset.x = screenSize.x;
		if (transform.localPosition.y <= -screenSize.y * 0.5f)
			wrappingOffset.y += screenSize.y;
		transform.localPosition += (Vector3)wrappingOffset;
	}

	void Update() {
		if (!captured) {
			model.localRotation = Quaternion.Euler(new Vector3(270, Time.time * 30, 0));
			model.localPosition = Vector3.up * (0.2f + 0.1f * Mathf.Sin(Time.time * 2));
			model.localScale = Vector3.Lerp(model.localScale, baseScale, 10.0f * Time.deltaTime);
		} else {
			var colorWithoutAlpha = material.color;
			colorWithoutAlpha.a = 0;
			material.color = Color.Lerp(material.color, colorWithoutAlpha, 10.0f * Time.deltaTime);
			model.localScale = Vector3.Lerp(model.localScale, baseScale * 4, 10.0f * Time.deltaTime);
			var positionInFront = model.localPosition + Vector3.back * 4.0f;
			model.localPosition =  Vector3.Lerp(model.localPosition, positionInFront, 10.0f * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (!captured) {
			var playerController = other.GetComponent<PlayerController>();
			if (playerController != null) {
				playerController.PlayPickup();
				scenario.Advance();
				captured = true;
				StartCoroutine(DelayedDisable());
			}
		}
	}
	
	IEnumerator DelayedDisable() {
		yield return new WaitForSeconds(1);
		gameObject.SetActive(false);
	}
}
