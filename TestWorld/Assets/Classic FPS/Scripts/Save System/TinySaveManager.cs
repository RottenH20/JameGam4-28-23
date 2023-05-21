using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinySaveManager : MonoBehaviour
{
	[SerializeField] private string fileName = "HellishBattle.tss"; // file to save with the specified resolution
	[SerializeField] private bool dontDestroyOnLoad; // the object will move from one scene to another (you only need to add it once)

	void Awake()
	{
		TinySaveSystem.Initialize(fileName);
		if (dontDestroyOnLoad) DontDestroyOnLoad(transform.gameObject);
	}

	void OnApplicationQuit()
	{
		TinySaveSystem.SaveToDisk();
	}
}