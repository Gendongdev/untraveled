using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

	[System.NonSerialized]
	Material material;

	void Start () {
		material = GetComponent<MeshRenderer>().material;
	}
	
	void Update () {
		material.color = new Color(1, 1, 1, 0.2f + 0.3f * Mathf.PerlinNoise(transform.position.x * transform.position.y, Time.time * 3.0f));
	}
}
