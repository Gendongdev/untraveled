using UnityEngine;
using System.Collections;

public class ExtensiblePlatformController : MonoBehaviour {

	public bool IsExtended {
		get { return isExtended; }
		set {
			IsExtended = value;
			if (!IsExtended) {
				var wrappingOffset = Vector2.zero;
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
		}
	}

	[System.NonSerialized]
	private bool isExtended = true;

	[System.NonSerialized]
	public Vector2 screenSize;

	[System.NonSerialized]
	public Vector2 halfSize;

	void Start() {
		halfSize = Vector3.Scale(GetComponent<BoxCollider>().size, transform.localScale) * 0.5f;
	}
}
