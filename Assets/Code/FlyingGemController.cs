using UnityEngine;
using System.Collections;

public class FlyingGemController : MonoBehaviour {

	public ScenarioController scenario;
	public Transform model;

	[System.NonSerialized]
	Vector3 baseScale;
	[System.NonSerialized]
	Material material;
	[System.NonSerialized]
	bool captured;

	void Start() {
		baseScale = model.transform.localScale;
		model.transform.localScale = Vector3.zero;
		material = model.GetComponent<MeshRenderer>().material;
		captured = false;
	}
	
	void OnEnable() {
		material = model.GetComponent<MeshRenderer>().material;
	}
	
	void Update() {
		if (!captured) {
			model.localRotation = Quaternion.Euler(new Vector3(270, Time.time * 30, 0));
			model.localPosition = Vector3.up * (-0.3f + 0.1f * Mathf.Sin(Time.time * 2));
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
