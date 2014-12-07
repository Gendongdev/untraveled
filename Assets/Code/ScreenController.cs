using UnityEngine;
using System.Collections;

public class ScreenController : MonoBehaviour {
	public Vector2 size;

	void Start() {
		Screen.showCursor = false;
	}

	void Update() {
		if (Input.GetButtonDown("Quit"))
			Application.Quit();
	}
}
