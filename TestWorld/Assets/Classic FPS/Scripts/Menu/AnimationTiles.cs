using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTiles : MonoBehaviour
{
	public int materialIndex = 0;
	public MeshRenderer AnimatedMeshRenderer;

	public float TimeBetween = .1f;

	public int TilesInTextureY = 8;

	Material MoveableMaterial;
	int frame = 0;
	float timer;

	public void Start()
	{
		MoveableMaterial = AnimatedMeshRenderer.materials[materialIndex];
		// Main Texture
		MoveableMaterial.SetTextureScale("_MainTex", new Vector2(1f, 1f / TilesInTextureY));
		// Normal Map
		MoveableMaterial.SetTextureScale("_BumpMap", new Vector2(1f, 1f / TilesInTextureY));
		// Occlusion
		MoveableMaterial.SetTextureScale("_OcclusionMap", new Vector2(1f, 1f / TilesInTextureY));
		// Emission
		MoveableMaterial.SetTextureScale("_EmissionMap", new Vector2(1f, 1f / TilesInTextureY));
	}

	public void Update()
	{
		timer += Time.deltaTime;

		if (timer > TimeBetween)
		{
			frame++;
			MoveableMaterial.SetTextureOffset("_MainTex", new Vector2(1f, (1f / TilesInTextureY) * frame));
			MoveableMaterial.SetTextureOffset("_BumpMap", new Vector2(1f, (1f / TilesInTextureY) * frame));
			MoveableMaterial.SetTextureOffset("_OcclusionMap", new Vector2(1f, (1f / TilesInTextureY) * frame));
			MoveableMaterial.SetTextureOffset("_EmissionMap", new Vector2(1f, (1f / TilesInTextureY) * frame));
			timer = 0;
		}
	}
}
