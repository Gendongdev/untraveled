using UnityEngine;
using System.Collections;

public class EndBackgroundController : MonoBehaviour {

	[System.NonSerialized]
	Material material;
	[System.NonSerialized]
	float opacity;
	
	void Start () {
		material = GetComponent<MeshRenderer>().material;
		opacity = 0;
	}
	
	void Update () {
		material.color = new Color(0, 0, 0, opacity);
		opacity += Time.deltaTime;
	}
}
