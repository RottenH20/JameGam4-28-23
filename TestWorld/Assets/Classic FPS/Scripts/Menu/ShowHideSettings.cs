using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideSettings : MonoBehaviour
{
    public string SaveName;
    public int showWhen;

    void Start()
    {
        if (TinySaveSystem.GetInt(SaveName) == showWhen)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
