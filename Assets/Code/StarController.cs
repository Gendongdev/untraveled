using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

	[System.NonSerialized]
	Material material;

	[System.NonSerialized]
	Vector3 baseScale;

	void Start () {
		material = GetComponent<MeshRenderer>().material;
		baseScale = transform.localScale;
	}
	
	void Update () {
		material.color = new Color(1, 1, 1, 0.2f + 0.3f * Mathf.PerlinNoise(transform.position.x * transform.position.y, Time.time * 3.0f));
		transform.localScale = baseScale * (0.7f + 0.6f * Mathf.PerlinNoise(transform.position.x * transform.position.y, Time.time));
	}
}
