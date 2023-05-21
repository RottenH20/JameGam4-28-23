using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbilityScript : ScriptableObject
{
    public Sprite Icon;
    public LocalizedString Name;
    public LocalizedString Description;
    [Line("Cooldown")]
    public float cooldown = 15f;

    virtual public void ActivateAbility()
    {

    }
}
