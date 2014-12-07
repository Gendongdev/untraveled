using UnityEngine;
using System.Collections;

public class EndController : MonoBehaviour {

	public PlayerController player;
	public AudioSource wind;

	[System.NonSerialized]
	Material material;
	[System.NonSerialized]
	float opacity;
	
	void Start () {
		material = GetComponent<MeshRenderer>().material;
		opacity = 0;
		StartCoroutine(WaitAndStop());
	}

	IEnumerator WaitAndStop() {
		yield return new WaitForSeconds(1);
		player.enabled = false;
	}
	
	void Update () {
		wind.volume = Mathf.Lerp(wind.volume, 0.10f, 0.5f * Time.deltaTime);
		material.color = new Color(1, 0.99f, 0.95f, opacity * (0.4f + 0.6f * Mathf.PerlinNoise(transform.position.x * transform.position.y, Time.time * 3.0f)));
		opacity += 0.5f * Time.deltaTime;
		player.transform.localPosition = Vector3.Lerp(player.transform.localPosition, Vector3.back * 3.5f, 10.0f * Time.deltaTime);
		player.model.localRotation = Quaternion.RotateTowards(player.model.localRotation,
		                                                      Quaternion.LookRotation(Vector3.back), 5.0f * Time.deltaTime);
	}
}
