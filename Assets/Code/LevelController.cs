using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

	public ScreenController screen;
	public PlayerController player;

	[System.NonSerialized]
	Vector2 currentPosition;

	void Start() {
		currentPosition = Vector2.zero;
		foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
			meshRenderer.gameObject.AddComponent<DuplicateRender>().size = screen.size;
	}

	void FixedUpdate () {
		currentPosition -= player.movement;

		if (currentPosition.x >= screen.size.x * 0.5f)
			currentPosition.x -= screen.size.x;
		if (currentPosition.y >= screen.size.y * 0.5f)
			currentPosition.y -= screen.size.y;
		if (currentPosition.x <= -screen.size.x * 0.5f)
			currentPosition.x += screen.size.x;
		if (currentPosition.y <= -screen.size.y * 0.5f)
			currentPosition.y += screen.size.y;

		transform.localPosition = currentPosition;
	}
}
