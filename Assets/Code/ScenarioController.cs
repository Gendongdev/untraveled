using UnityEngine;
using System.Collections.Generic;

public class ScenarioController : MonoBehaviour {

	public List<GameObject> activables;

	[System.NonSerialized]
	private int nextStep;

	void Start () {
		foreach (var activable in activables)
			activable.SetActive(false);
		nextStep = 0;
		Advance();
	}

	public void Advance() {
		if (nextStep == activables.Count)
			Debug.LogWarning("Scenario finished");
		else
			activables[nextStep++].SetActive(true);
	}
}
