using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BaseWeaponScript : MonoBehaviour
{

    [Line("UI Settings")]
    public LocalizedString weaponName;

    private StringLocalization localization;
    private string weaponNameKey;

    public Sprite weaponIcon;

    public virtual void PrimaryModeFunction()
    {

    }

    public virtual void SecondaryModeFunction()
    {

    }

    public virtual void ReloadFunction()
    {

    }

    public virtual void UpdateLeftAmmo()
    {

    }

    private void OnValidate()
    {
        if (weaponName.localization == null && localization != null) { weaponName.localization = localization; }
        if (weaponName.key == "" && weaponNameKey != "") { weaponName.key = weaponNameKey; }
    }
}




public enum WeaponTypeShooting
{
    Hitscan = 0,
    Projectile = 1
}
public enum PrimaryFireMode
{
    Single = 0,
    //Burst = 1,
    Automatic = 2
}
public enum SecondaryFireMode
{
    None = 0,
    Aim = 1
}

public enum RangeWeaponShootType
{
    Single = 0,
    Burst = 1,
    Automatic = 2,
    Charge = 3,
    Laser = 4
}

public enum BulleType
{
    Normal = 0,
    Explosion = 1
}

public enum AlternativeMelleType
{
    None = 0,
    AlternativeAttack = 1,
    Block = 2
}

public enum AnimationWeaponType
{
    SingleWeapon = 0,
    DualWeapon = 1
}

