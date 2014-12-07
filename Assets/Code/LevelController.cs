using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

	public ScreenController screen;
	public PlayerController player;

	[System.NonSerialized]
	Vector2 currentPosition;

	[System.NonSerialized]
	ExtensiblePlatformController[] extensiblePlatforms;

	void Start() {
		currentPosition = Vector2.zero;
		foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
			meshRenderer.gameObject.AddComponent<DuplicateRender>().screenSize = screen.size;
		extensiblePlatforms = GetComponentsInChildren<ExtensiblePlatformController>();
		foreach (var extensiblePlatform in extensiblePlatforms)
			extensiblePlatform.screenSize = screen.size;
	}

	void OnEnable() {
		extensiblePlatforms = GetComponentsInChildren<ExtensiblePlatformController>();
	}

	void FixedUpdate () {
		var screenHalfSize = screen.size * 0.5f;
		currentPosition -= player.movement;

		var wrappingOffset = Vector2.zero;
		if (currentPosition.x >= screenHalfSize.x)
			wrappingOffset.x = -screen.size.x;
		if (currentPosition.y >= screenHalfSize.y)
			wrappingOffset.y = -screen.size.y;
		if (currentPosition.x <= -screenHalfSize.x)
			wrappingOffset.x = screen.size.x;
		if (currentPosition.y <= -screenHalfSize.y)
			wrappingOffset.y = screen.size.y;

		currentPosition += wrappingOffset;

		foreach (var extensiblePlatform in extensiblePlatforms) {
			if (extensiblePlatform.IsExtended) {
				var availableHalfSize = extensiblePlatform.halfSize - screenHalfSize;

				var leftOffset = Mathf.Min(0,  extensiblePlatform.transform.position.x - player.movement.x - availableHalfSize.x);
				var rightOffset = Mathf.Max(0, extensiblePlatform.transform.position.x - player.movement.x + availableHalfSize.x);
				var bottomOffset = Mathf.Min(0, extensiblePlatform.transform.position.y - player.movement.y - availableHalfSize.y);
				var topOffset = Mathf.Max(0, extensiblePlatform.transform.position.y - player.movement.y + availableHalfSize.y);

				extensiblePlatform.transform.localPosition -= new Vector3(
					leftOffset + rightOffset + wrappingOffset.x,
					bottomOffset + topOffset + wrappingOffset.y, 0);
			}
		}

		RaycastHit hitInfo;
		Vector3 movement3 = player.movement;
		foreach (var flyingGem in GetComponentsInChildren<FlyingGemController>()) {
			// XXX: this is ugly...
			if (movement3.magnitude == 0 || !Physics.SphereCast(flyingGem.transform.position, 0.3f, movement3.normalized, out hitInfo, player.movement.magnitude))
					flyingGem.transform.localPosition += (Vector3) (player.movement - wrappingOffset);
		}
		transform.localPosition = currentPosition;
	}
}
