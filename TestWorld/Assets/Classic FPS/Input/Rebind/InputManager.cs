using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public TinyInput input;
    public static InputManager Instance;

    public void Awake()
    {

        input = new TinyInput();

        var rebinds = PlayerPrefs.GetString("rebinds");
        input.asset.LoadBindingOverridesFromJson(rebinds);

        // Set Static Object
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnDisable()
    {
        var rebinds = input.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    void OnLevelWasLoaded()
    {
        var rebinds = input.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

}
