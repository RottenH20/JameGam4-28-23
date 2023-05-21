/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.3.0a
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class ProjectileWeapon : BaseWeaponScript
{
    [Header("Weapon Settings")]
    public PrimaryFireMode PrimaryMode;
    public SecondaryFireMode SecondaryMode;

    [Header("Normal Mode")]
    public float weaponSpread;
    public float baseFOV;
    [Header("Aim Mode")]
    public float weaponAimSpread;
    public float aimFOV;

    [Header("AMMO & DAMAGE")]
    public float rocketForce;
    public float explosionRadius;
    public float explosionDamage;
    public AnimationCurve explosionDamageCurve;
    public float explosionShakeDistance;
    public float timeBetweenShot;
    public int magazineSize;
    public AmmoType ammoType;
    public LayerMask explosionLayerMask;

    [Header("SOUND")]
    public AudioClip shotSound;
    public AudioClip reloadSound;
    public AudioClip emptyGunSound;
    public AudioClip explosionSound;

    [Header("UI")]
    public Sprite UISprite;
    public TMP_Text ammoText;
    public Image WeaponUIImage;

    [Header("PREFABS")]
    public GameObject rocket;
    public GameObject explosion;
    public GameObject spawnPoint;

    AudioSource source;

    int ammoLeft;
    int ammoClipLeft = 666;
    bool isReloading;
    float timer;
    bool isShot;
    float currentSpread;
    bool aim = false;
    Animator anim;
    Transform mainCamera;

    //GameObject crosshair;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        if (ammoType != AmmoType.InfinityAmmo) ammoLeft = GameManager.Instance.AmmoLeft(ammoType);
        if (magazineSize > 1) ammoClipLeft = magazineSize;
    }

    void OnEnable()
    {
        isReloading = false;
        mainCamera = Camera.main.transform;
        if (ammoType != AmmoType.InfinityAmmo) ammoLeft = GameManager.Instance.AmmoLeft(ammoType);
        WeaponUIImage.sprite = UISprite;
        timer = 666;
    }

    void OnDisable()
    {
        //crosshair.SetActive(true);
    }

    void Update()
    {
        if (ammoType != AmmoType.InfinityAmmo && magazineSize < 1) ammoText.text = ammoLeft.ToString();
        else if (ammoType != AmmoType.InfinityAmmo) ammoText.text = ammoClipLeft + "<size=8> / " + ammoLeft;
        else if (ammoType == AmmoType.InfinityAmmo && magazineSize > 0) ammoText.text = ammoClipLeft + "<size=8> / Infinity";
        else ammoText.text = "Infinity";

        timer += Time.deltaTime;

        if (PrimaryMode == PrimaryFireMode.Single && Input.GetKeyDown(KeyCode.Mouse0) && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft > 0) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } }
        if (PrimaryMode == PrimaryFireMode.Automatic && Input.GetKey(KeyCode.Mouse0) && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft > 0) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } }
        if (Input.GetKeyDown(KeyCode.R) && isReloading == false && ammoClipLeft != magazineSize) { Reload(); }

        if (Input.GetKey(KeyCode.Mouse1) && !Application.isMobilePlatform && SecondaryMode == SecondaryFireMode.Aim && isReloading == false) { anim.SetBool("aim", true); aim = true; } else { anim.SetBool("aim", false); aim = false; }

        // Change FOV
        if (aim) { mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, aimFOV, .5f); currentSpread = weaponAimSpread; }
        else { mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, baseFOV, .5f); currentSpread = weaponSpread; }
    }
    public void FixedUpdate()
    {
        if (isShot == true && ammoClipLeft > 0 && isReloading == false)
        {
            source.PlayOneShot(shotSound);
            StartCoroutine("shot");
            isShot = false;

            if (magazineSize >= 1) ammoClipLeft--;
            else if (ammoType != AmmoType.InfinityAmmo) { GameManager.Instance.AddAmmo(ammoType, -1); ammoLeft--; }
            GameObject rocketInstantiated = (GameObject)Instantiate(rocket, spawnPoint.transform.position, Quaternion.identity);
            rocketInstantiated.GetComponent<BulletScript>().damage = explosionDamage;
            rocketInstantiated.GetComponent<BulletScript>().damageCurve = explosionDamageCurve;
            rocketInstantiated.GetComponent<BulletScript>().radius = explosionRadius;
            rocketInstantiated.GetComponent<BulletScript>().explosionSound = explosionSound;
            rocketInstantiated.GetComponent<BulletScript>().layerMask = explosionLayerMask;
            rocketInstantiated.GetComponent<BulletScript>().explosion = explosion;
            rocketInstantiated.GetComponent<BulletScript>().shakeRadius = explosionShakeDistance;
            Rigidbody rocketRb = rocketInstantiated.GetComponent<Rigidbody>();

            rocketRb.AddForce(Camera.main.transform.forward * rocketForce, ForceMode.Impulse);
            timer = 0;
        }
        else if (isShot == true && ammoClipLeft <= 0 && isReloading == false)
        {
            isShot = false;
            ReloadVoid();
        }
    }

    void ReloadVoid()
    {
        // Function responsible for reloading weapons
        int bulletsToReload = magazineSize - ammoClipLeft;
        if (magazineSize > 0)
        {
            if (ammoType != AmmoType.InfinityAmmo)
            {
                if (ammoLeft >= bulletsToReload)
                {
                    StartCoroutine("ReloadWeapon");
                    ammoLeft -= bulletsToReload;
                    if (magazineSize > 1) ammoClipLeft = magazineSize;
                    GameManager.Instance.SetNewAmmoAmount(ammoType, ammoLeft);
                }
                else if (ammoLeft < bulletsToReload && ammoLeft > 0)
                {
                    StartCoroutine("ReloadWeapon");
                    if (magazineSize > 1) ammoClipLeft += ammoLeft;
                    ammoLeft = 0;
                    GameManager.Instance.SetNewAmmoAmount(ammoType, 0);
                }
                else if (ammoLeft <= 0)
                {
                    if (timer > timeBetweenShot)
                    {
                        source.PlayOneShot(emptyGunSound);
                        timer = 0;
                    }
                }
            }
            else
            {
                StartCoroutine("ReloadWeapon");
                if (magazineSize > 1) ammoClipLeft = magazineSize;
            }
        }
    }

    // Function Play Sound Of Reloading
    IEnumerator Reload()
    {
        isReloading = true;
        source.PlayOneShot(reloadSound);
        anim.Play("Reload", -1, 0f);
        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(2);
        isReloading = false;
    }

    // Function while firing changes weapon art to 0.1 seconds
    IEnumerator shot()
    {
        if (Input.GetKey(KeyCode.Mouse1) && SecondaryMode == SecondaryFireMode.Aim) anim.Play("Aim_Shot", -1, 0f);
        else anim.Play("Shot", -1, 0f);
        yield return new WaitForSeconds(0.1f);
    }

    public override void PrimaryModeFunction()
    {
        if (isReloading == false && timer > timeBetweenShot)
        {
            if (magazineSize <= 0) { if (ammoLeft > 0) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; }
        }
    }
    public override void SecondaryModeFunction()
    {
        if (SecondaryMode == SecondaryFireMode.Aim && isReloading == false) { anim.SetBool("aim", true); aim = true; } else { anim.SetBool("aim", false); aim = false; }
    }

    public override void ReloadFunction()
    {
        Reload();
    }
}