using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class ConsoleController : MonoBehaviour
{
    [Line("Cheat Console")]
    public TMP_InputField commandText;
    public TMP_Text commandInfo;

    [Line("Cheat Console")]
    public string currentString = "";
    public float ClearDataAfter;
    bool Timer = false;
    private TinyInput tinyInput;

    // Private
    public List<CheatCommandBase> FlyCommandList = new List<CheatCommandBase> {
        new CheatCommand("FULLCLIP", "Gives the player the maximum amount of Ammo", "", () =>
        {
            GameManager.Instance.AddAmmo(AmmoType.Bullet, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Shell, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Clip, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Grenade, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Rocket, 9999);
            GameManager.Instance.AddAmmo(AmmoType.EnergyCell, 9999);
            GameManager.Instance.AddAmmo(AmmoType.HandGrenade, 9999);

            // Update UI
            WeaponSwitch container = Camera.main.transform.parent.GetComponentInChildren(typeof(WeaponSwitch)) as WeaponSwitch;
            if (container != null) { container.actualWeapon.UpdateLeftAmmo(); }
        }),
        new CheatCommand("IWASGOD", "Max Health and Armor", "", () =>
        {
            GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().AddHealth(9999, true);
            GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().AddArmor(9999, true);
        })
    };

    public List<CheatCommandBase> commandList = new List<CheatCommandBase> {
        new CheatCommand("kill_enemy", "Kills all enemies on the map", "kill_enemy", () =>
        {
            Enemy[] buttonObjs = FindObjectsOfType<Enemy>();

            for(int i=0; i< buttonObjs.Length; i++)
            {
                buttonObjs[i].TakeDamage(99999);
            }
        }),
        new CheatCommand("max_health", "Restores the player's maximum amount of life", "max_health", () =>
        {
            GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().AddHealth(9999, true);
        }),
        new CheatCommand("max_armor", "Restores the player's maximum amount of armor", "max_armor", () =>
        {
            GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().AddArmor(9999, true);
        }),
        new CheatCommand("max_ammo", "Gives the player the maximum amount of Ammo", "max_ammo", () =>
        {
            GameManager.Instance.AddAmmo(AmmoType.Bullet, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Shell, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Clip, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Grenade, 9999);
            GameManager.Instance.AddAmmo(AmmoType.Rocket, 9999);
            GameManager.Instance.AddAmmo(AmmoType.EnergyCell, 9999);
            GameManager.Instance.AddAmmo(AmmoType.HandGrenade, 9999);

            // Update UI
            WeaponSwitch container = Camera.main.transform.parent.GetComponentInChildren(typeof(WeaponSwitch)) as WeaponSwitch;
            if (container != null) { container.actualWeapon.UpdateLeftAmmo(); }
        }),
        new CheatCommand<int>("set_health", "Allows you to set any number of lives from 0 to the maximum level", "set_health <color=#17713D>[0-200]</color>", (x) =>
        {
            float currentHealth = GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().health;
            GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().AddHealth(x-currentHealth, true);
        }),
        new CheatCommand<int>("set_armor", "Allows you to set any number of armor from 0 to the maximum level", "set_armor <color=#17713D>[0-100]</color>", (x) =>
        {
            float currentArmor = GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().armor;
            GameManager.Instance.transform.parent.GetComponent<PlayerHealth>().AddArmor(x-currentArmor, true);
        }),
    };
    bool showConsole;

    private void Awake()
    {
        tinyInput = InputManager.Instance.input;
        var rebinds = PlayerPrefs.GetString("rebinds");
        tinyInput.asset.LoadBindingOverridesFromJson(rebinds);

        tinyInput.Menu.Console.performed += ctx => { showConsole = !showConsole; GameManager.Instance.paused = showConsole; GameManager.Instance.CheckPauseGame();  transform.Find("UI").gameObject.SetActive(showConsole); };
    }

    public void OnEnable() { tinyInput.Enable(); }
    public void OnDisable() { tinyInput.Disable(); }

    public void Update()
    {
        commandInfo.text = "";
        for (int i = 0; i < commandList.Count; i++)
        {
            CheatCommandBase commandBase = commandList[i];
            if (commandList[i].commandID.IndexOf(commandText.text, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                commandInfo.text += "<color=#C8C8C8>" + commandList[i].commandFormat + " <color=#555>-<color=#888> " + commandList[i].commandDescription + "<br>";
            }
        }

        // Fly Cheat
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) 
        {
            if (Input.GetKeyDown(vKey))
            {
                currentString += vKey;
                Reset_Data();
            }
        }


        CheckCheat(currentString);
        if (currentString == "")
        {
            Timer = false;
        }
        else
        {
            Timer = true;
        }
        if (Timer)
        {
            if (ClearDataAfter > 0)
            {
                ClearDataAfter -= Time.deltaTime;
            }
            else
            {
                Reset_Data();
                Timer = false;
                currentString = "";
            }
        }
    }

    private void Reset_Data()
    {
        ClearDataAfter = 2;
    }
    private bool CheckCheat(string _input)
    {
        foreach (CheatCommand code in FlyCommandList)
        {
            if (_input == code.commandID)
            {
                if (code.commandID != "")
                {
                    code.Invoke();
                    Reset_Data();
                    currentString = "";
                    return true;
                }
            }
        }

        return false;
    }

    public void HandleInput()
    {
        string[] properties = commandText.text.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            CheatCommandBase commandBase = commandList[i] as CheatCommandBase;
            if (commandText.text.Contains(commandBase.commandID))
            {
                if (commandList[i] as CheatCommand != null)
                {
                    (commandList[i] as CheatCommand).Invoke();
                }
                else if (commandList[i] as CheatCommand<int> != null)
                {
                    (commandList[i] as CheatCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
        commandText.text = "";
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ConsoleController))]
public class ConsoleControllerEditor : Editor
{
    //SerializedProperty lookAtPoint;

    void OnEnable()
    {
        //lookAtPoint = serializedObject.FindProperty("lookAtPoint");
    }

    public override void OnInspectorGUI()
    {
        ConsoleController loot = (ConsoleController)target;

        serializedObject.Update();
        base.OnInspectorGUI();

        GUILayout.BeginVertical("box");

        GUILayout.BeginVertical("Toolbar");
        GUILayout.Label("Console Cheats");
        GUILayout.EndVertical();
        for (int j = 0; j < loot.commandList.Count; j++)
        {
            GUILayout.Label(loot.commandList[j].commandID + " - " + loot.commandList[j].commandDescription);
        }

        GUILayout.Space(5);

        GUILayout.BeginVertical("Toolbar");
        GUILayout.Label("Fly Cheats");
        GUILayout.EndVertical();
        for (int j = 0; j < loot.FlyCommandList.Count; j++)
        {
            GUILayout.Label(loot.FlyCommandList[j].commandID + " - " + loot.FlyCommandList[j].commandDescription);
        }
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif