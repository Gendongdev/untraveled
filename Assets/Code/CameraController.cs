using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	[Range(1, 180)]
	public float verticalFov = 38.0f;

	[Range(0.1f, 10)]
	public float targetRatio = 1.6f;

	void OnPreCull() {
		var screenRatio = ((float)Screen.width) / Screen.height;
		if (screenRatio < targetRatio) {
			camera.fieldOfView = verticalFov / screenRatio * targetRatio;
		} else {
			camera.fieldOfView = verticalFov;
		}
	}
}
