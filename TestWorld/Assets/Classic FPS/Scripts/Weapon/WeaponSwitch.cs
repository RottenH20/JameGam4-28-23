/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.3.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour
{
    [Line("Weapon List")]
    public int initialWeapon;
    public BaseWeaponScript actualWeapon;
    public BaseWeaponScript noWeaponObject;
    public List<Transform> weapons;
    public bool autoFill;

    [Line("Ammo UI")]
    public GameObject AmmoText;
    public Image AmmoImage;
    [Line("Ammo Sprite")]
    public Sprite BulletIcon;
    public Sprite ShellIcon;
    public Sprite ClipIcon;
    public Sprite GrenadetIcon;
    public Sprite RocketIcon;
    public Sprite EnergyCellIcon;
    public Sprite HandGrenadeIcon;
    public Sprite InfinityIcon;

    // Privates
    [HideInInspector] public bool SafeZone;
    private TinyInput tinyInput;
    [HideInInspector] public int selectedWeapon;
    float changeTimer;

    private void Awake()
    {
        if (autoFill)
        {
            weapons.Clear();
            foreach (Transform weapon in transform) 
            {
                if (noWeaponObject.name != weapon.gameObject.name) { weapons.Add(weapon); }
            }
        }
        tinyInput = new TinyInput();
    }
    public void OnEnable() { tinyInput.Enable(); }
    public void OnDisable() { tinyInput.Disable(); }

    void Start() {
        selectedWeapon = initialWeapon % weapons.Count;
        UpdateWeapon();
    }

    void Update() {
        if (changeTimer > 0)
        {
            changeTimer -= Time.deltaTime;
        }

        if (!SafeZone)
        {
            // Scroll to change weapons
            if (tinyInput.Weapon.Select_Weapon.ReadValue<float>() > 0 && changeTimer <= 0) // Next
            {
                actualWeapon.StopAllCoroutines();
                selectedWeapon++;
                if (selectedWeapon == weapons.Count) { selectedWeapon = 0; }
                UpdateWeapon();
                changeTimer = 1 / 15;
            }
            if (tinyInput.Weapon.Select_Weapon.ReadValue<float>() < 0 && changeTimer <= 0) // Previous
            {
                actualWeapon.StopAllCoroutines();
                selectedWeapon -= 1;
                if (selectedWeapon < 0) { selectedWeapon = weapons.Count - 1; }
                UpdateWeapon();
                changeTimer = 1 / 15;
            }

            // Keys to change weapons
            if (tinyInput.Weapon.Weapon_Select_1.ReadValue<float>() == 1)
                selectedWeapon = 0;
            if (tinyInput.Weapon.Weapon_Select_2.ReadValue<float>() == 1)
                selectedWeapon = 1;
            if (tinyInput.Weapon.Weapon_Select_3.ReadValue<float>() == 1)
                selectedWeapon = 2;
            if (tinyInput.Weapon.Weapon_Select_4.ReadValue<float>() == 1)
                selectedWeapon = 3;
            if (tinyInput.Weapon.Weapon_Select_5.ReadValue<float>() == 1)
                selectedWeapon = 4;
            if (tinyInput.Weapon.Weapon_Select_6.ReadValue<float>() == 1)
                selectedWeapon = 5;
            if (tinyInput.Weapon.Weapon_Select_7.ReadValue<float>() == 1)
                selectedWeapon = 6;
            if (tinyInput.Weapon.Weapon_Select_8.ReadValue<float>() == 1)
                selectedWeapon = 7;
            if (tinyInput.Weapon.Weapon_Select_9.ReadValue<float>() == 1)
                selectedWeapon = 8;

        }

        UpdateWeapon();
        
        if (actualWeapon.TryGetComponent<MelleWeapon>(out MelleWeapon melle))
        {
            if (melle.anim.GetBool("Block") == true)
            {
                transform.parent.GetComponent<PlayerHealth>().WeaponBlockingChange = actualWeapon.GetComponent<MelleWeapon>().BlockingDamageChange;
                transform.parent.GetComponent<PlayerHealth>().WeaponDamageReductionPercentage = actualWeapon.GetComponent<MelleWeapon>().DamageReductionPercentage;
            }
            else
            {
                transform.parent.GetComponent<PlayerHealth>().WeaponBlockingChange = 0;
                transform.parent.GetComponent<PlayerHealth>().WeaponDamageReductionPercentage = 0;
            }
            AmmoText.SetActive(false);
        }
        else
        {
            transform.parent.GetComponent<PlayerHealth>().WeaponBlockingChange = 0;
            transform.parent.GetComponent<PlayerHealth>().WeaponDamageReductionPercentage = 0;
            AmmoText.SetActive(true);

            if (actualWeapon.TryGetComponent<RangeWeapon>(out RangeWeapon range))
            {
                AmmoImage.sprite = UpdateAmmoIcon(range.ammoType);
            }
            else if (actualWeapon.TryGetComponent<ThrowableWeapon>(out ThrowableWeapon throwable))
            {
                AmmoImage.sprite = UpdateAmmoIcon(throwable.ammoType);
            }
        }
        //actualWeapon.UpdateLeftAmmo();
    }

    Sprite UpdateAmmoIcon(AmmoType type)
    {
        if (type == AmmoType.Bullet) return BulletIcon;
        if (type == AmmoType.Shell) return ShellIcon;
        if (type == AmmoType.Clip) return ClipIcon;
        if (type == AmmoType.Grenade) return GrenadetIcon;
        if (type == AmmoType.Rocket) return RocketIcon;
        if (type == AmmoType.EnergyCell) return EnergyCellIcon;
        if (type == AmmoType.HandGrenade) return HandGrenadeIcon;

        return InfinityIcon;
    }

    void UpdateWeapon()
    {
        if (SafeZone)
        {
            for (int i = 0; i < weapons.Count; i++) { weapons[i].gameObject.SetActive(false); }
            noWeaponObject.gameObject.SetActive(true);
            actualWeapon = noWeaponObject;
        }
        else
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                if (i == selectedWeapon)
                {
                    weapons[i].gameObject.SetActive(true);
                    actualWeapon = weapons[i].GetComponent<BaseWeaponScript>();
                }
                else
                {
                    weapons[i].gameObject.SetActive(false);
                }
            }
            noWeaponObject.gameObject.SetActive(false);
        }
    }

    public void UINextWeapon()
    {
        selectedWeapon = (selectedWeapon + 1) % weapons.Count;
    }

    public void UIPreviousWeapon()
    {
        selectedWeapon = Mathf.Abs(selectedWeapon - 1) % weapons.Count;
    }

    public void Primary() { actualWeapon.PrimaryModeFunction(); }
    public void Secoundary() { actualWeapon.SecondaryModeFunction(); }
    public void Reload() { actualWeapon.ReloadFunction(); }
}
