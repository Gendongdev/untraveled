using UnityEngine;
using System.Collections;

public class LastGemController : MonoBehaviour {

	public PlayerController player;
	public ExtensiblePlatformController extensiblePlatformController;
	
	[System.NonSerialized]
	bool waitForActivation;
	[System.NonSerialized]
	bool animatePlatform;

	void OnDisable() {
		waitForActivation = true;
	}

	void OnEnable() {
		animatePlatform = false;
		if (waitForActivation)
			StartCoroutine(EnableDelayed());
	}

	IEnumerator EnableDelayed() {
		yield return new WaitForSeconds(0.6f);
		player.PlayMoveRock();
		extensiblePlatformController.enabled = true;
		var duplicateRenderers = extensiblePlatformController.gameObject.GetComponentsInChildren<DuplicateRender>();
		foreach (var duplicateRenderer in duplicateRenderers)
			duplicateRenderer.enabled = false;
		animatePlatform = true;
	}
	
	void Update() {
		if (animatePlatform) {
			var positionInFront = extensiblePlatformController.transform.localPosition;
			positionInFront.z = -2.0f;
			extensiblePlatformController.transform.localPosition = Vector3.Lerp(
				extensiblePlatformController.transform.localPosition,
				positionInFront,
				Time.deltaTime);
		}
	}

}
