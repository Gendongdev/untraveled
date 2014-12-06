using UnityEngine;
using System.Collections;

public class DuplicateRender : MonoBehaviour {

	[System.NonSerialized]
	public Vector2 size;

	[System.NonSerialized]
	MeshFilter meshFilter;
	[System.NonSerialized]
	MeshRenderer meshRenderer;

	[System.NonSerialized]
	Matrix4x4 topLeftMatrix;
	Matrix4x4 topMatrix;
	Matrix4x4 topRightMatrix;
	Matrix4x4 leftMatrix;
	Matrix4x4 rightMatrix;
	Matrix4x4 bottomLeftMatrix;
	Matrix4x4 bottomMatrix;
	Matrix4x4 bottomRightMatrix;

	void Start() {
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();

		topLeftMatrix = Matrix4x4.TRS(new Vector3(-size.x, -size.y, 0), Quaternion.identity, Vector3.one);
		topMatrix = Matrix4x4.TRS(new Vector3(0, -size.y, 0), Quaternion.identity, Vector3.one);
		topRightMatrix = Matrix4x4.TRS(new Vector3(size.x, -size.y, 0), Quaternion.identity, Vector3.one);

		leftMatrix = Matrix4x4.TRS(new Vector3(-size.x, 0, 0), Quaternion.identity, Vector3.one);
		rightMatrix = Matrix4x4.TRS(new Vector3(size.x, 0, 0), Quaternion.identity, Vector3.one);

		bottomLeftMatrix = Matrix4x4.TRS(new Vector3(-size.x, size.y, 0), Quaternion.identity, Vector3.one);
		bottomMatrix = Matrix4x4.TRS(new Vector3(0, size.y, 0), Quaternion.identity, Vector3.one);
		bottomRightMatrix = Matrix4x4.TRS(new Vector3(size.x, size.y, 0), Quaternion.identity, Vector3.one);
	}

	void OnEnable() {
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
	}

	void Update () {
		var materialPropertyBlock = new MaterialPropertyBlock();
		var baseMatrix = transform.localToWorldMatrix;
		Graphics.DrawMesh(meshFilter.sharedMesh, topLeftMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);
		Graphics.DrawMesh(meshFilter.sharedMesh, topMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);
		Graphics.DrawMesh(meshFilter.sharedMesh, topRightMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);

		Graphics.DrawMesh(meshFilter.sharedMesh, leftMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);
		Graphics.DrawMesh(meshFilter.sharedMesh, rightMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);

		Graphics.DrawMesh(meshFilter.sharedMesh, bottomLeftMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);
		Graphics.DrawMesh(meshFilter.sharedMesh, bottomMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);
		Graphics.DrawMesh(meshFilter.sharedMesh, bottomRightMatrix * baseMatrix, meshRenderer.sharedMaterial, 0, null, 0, materialPropertyBlock, meshRenderer.receiveShadows, meshRenderer.castShadows);
	}
}
