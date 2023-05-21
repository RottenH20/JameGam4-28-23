using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class ThrowableWeapon : BaseWeaponScript
{
    [Line("Base Settings")]
    public float Damage;
    public float FOV = 60;
    public float explosionRadius = 2.5f;
    public AnimationCurve explosionDamageCurve;
    public float explosionShakeDistance = 5.5f;
    public AmmoType ammoType = AmmoType.HandGrenade;
    public LayerMask raycastMask;

    [Line("Base Throw")]
    public float timeBeforeExplosion = 3;
    public float timeBetweenThrow = .75f;
    public float throwableForce = 15;
    public float throwableActivatorTime = 24f / 60;
    public List<AnimationClip> BaseAttackAnimation;

    [Line("Alternative Throw")]
    public bool enableAlternativeThrow;
    [ShowIf("enableAlternativeThrow", false)] public float AlternativeTimeBeforeExplosion = 5;
    [ShowIf("enableAlternativeThrow", false)] public float AlternativetimeBetweenThrow = .75f;
    [ShowIf("enableAlternativeThrow", false)] public float AlternativeThrowableForce = 30;
    [ShowIf("enableAlternativeThrow", false)] public float AlternativeThrowableActivatorTime = 24f / 60;
    public List<AnimationClip> AlternativeAttackAnimation;

    [Line("Prefabs & Objects")]
    public GameObject projectileObject;
    public GameObject explosionPrefab;
    public GameObject spawnpointPrefab;

    [Line("UI")]
    public TMP_Text ammoText;
    public TMP_Text WeaponNameText;
    public Image WeaponUIImage;

    [Line("Sound")]
    public AudioClip throwSound;
    [ShowIf("enableAlternativeThrow")] public AudioClip throwAlternativeSound;
    public AudioClip explosionSound;

    [Line("Events")]
    public UnityEvent AdditionalBaseThrowFunction;
    public UnityEvent AdditionalAlternativeThrowFunction;

    // Privates
    AudioSource source;
    Animator anim;
    Transform mainCamera;

    float timer;
    int itemCount = -1;

    private TinyInput tinyInput;

    void Awake()
    {
        tinyInput = InputManager.Instance.input;

        // GetComponents
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        // If not Infinity Ammo
        if (ammoType != AmmoType.InfinityAmmo) itemCount = GameManager.Instance.AmmoLeft(ammoType);

        //
        tinyInput.Weapon.Fire.started += ctx => { if (timer > timeBetweenThrow) { if (itemCount != 0) { StartCoroutine("BaseAttack"); } else { /* Nothing */ } } };
        //
        tinyInput.Weapon.Aim.started += ctx => { if (enableAlternativeThrow && timer > AlternativeTimeBeforeExplosion) { if (itemCount != 0) { StartCoroutine("AlternativeAttack"); } else { /* Nothing */ } } };
    }

    void OnEnable()
    {
        mainCamera = Camera.main.transform;
        // If not Infinity Ammo
        if (ammoType != AmmoType.InfinityAmmo) itemCount = GameManager.Instance.AmmoLeft(ammoType);

        WeaponNameText.text = weaponName.localization.GetString(weaponName.key);

        WeaponUIImage.sprite = weaponIcon;
        // Ready to Shot
        timer = 666;

        WeaponNameText.text = weaponName.localization.GetString(weaponName.key);


        //tinyInput.Enable();
    }

    private void OnDisable()
    {
        //tinyInput.Disable();
    }

    void Update()
    {
        if (!GameManager.Instance.paused)
        {
            ammoText.text = itemCount.ToString();

            timer += Time.deltaTime;

            // Normal
            //if (Input.GetKeyDown(KeyCode.Mouse0) && timer > timeBetweenThrow && !Application.isMobilePlatform) { if (itemCount != 0) { StartCoroutine("BaseAttack"); } else { /* Nothing */ } }
            // Alternative
            //else if (enableAlternativeThrow && Input.GetKeyDown(KeyCode.Mouse1) && timer > AlternativeTimeBeforeExplosion && !Application.isMobilePlatform) { if (itemCount != 0) { StartCoroutine("AlternativeAttack"); } else { /* Nothing */ } }

            // FOV
            mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, FOV, .5f);
        }
    }

    IEnumerator BaseAttack()
    {
        anim.Play(BaseAttackAnimation[Random.Range(0, BaseAttackAnimation.Count)].name, -1, 0f);
        itemCount--;
        if (ammoType != AmmoType.InfinityAmmo) { GameManager.Instance.AddAmmo(ammoType, -1); }
        timer = 0;

        yield return new WaitForSeconds(throwableActivatorTime);

        source.PlayOneShot(throwSound);
        GenerateThrowable(throwableForce);
    }

    IEnumerator AlternativeAttack()
    {
        anim.Play(AlternativeAttackAnimation[Random.Range(0, AlternativeAttackAnimation.Count)].name, -1, 0f);
        itemCount--;
        if (ammoType != AmmoType.InfinityAmmo) { GameManager.Instance.AddAmmo(ammoType, -1); }
        timer = 0;

        yield return new WaitForSeconds(AlternativeThrowableActivatorTime);

        source.PlayOneShot(throwAlternativeSound);
        GenerateThrowable(AlternativeThrowableForce);
    }

    public void GenerateThrowable(float force)
    {
        GameObject projectileInstantiated = (GameObject)Instantiate(projectileObject, spawnpointPrefab.transform.position, Quaternion.identity);
        projectileInstantiated.GetComponent<BulletScript>().type = BulleType.Explosion;
        projectileInstantiated.GetComponent<BulletScript>().damage = Damage;
        projectileInstantiated.GetComponent<BulletScript>().damageCurve = explosionDamageCurve;
        projectileInstantiated.GetComponent<BulletScript>().radius = explosionRadius;
        projectileInstantiated.GetComponent<BulletScript>().explosionSound = explosionSound;
        projectileInstantiated.GetComponent<BulletScript>().timeBeforeExplosion = timeBeforeExplosion;
        projectileInstantiated.GetComponent<BulletScript>().layerMask = raycastMask;
        projectileInstantiated.GetComponent<BulletScript>().explosion = explosionPrefab;
        projectileInstantiated.GetComponent<BulletScript>().shakeRadius = explosionShakeDistance;

        var randomNumberX = Random.Range(-DynamicCrosshair.spread / 900, DynamicCrosshair.spread / 900);
        var randomNumberY = Random.Range(-DynamicCrosshair.spread / 900, DynamicCrosshair.spread / 900);
        var randomNumberZ = Random.Range(-DynamicCrosshair.spread / 900, DynamicCrosshair.spread / 900);

        Vector3 ve = new Vector3(randomNumberX, randomNumberY, randomNumberZ);

        Rigidbody rocketRb = projectileInstantiated.GetComponent<Rigidbody>();

        rocketRb.AddForce((Camera.main.transform.forward + ve) * force, ForceMode.Impulse);
    }

    IEnumerator ThrowItem(bool alternative)
    {
    
        yield return new WaitForSeconds(0);
    }

    public override void UpdateLeftAmmo()
    {
        if (ammoType != AmmoType.InfinityAmmo) itemCount = GameManager.Instance.AmmoLeft(ammoType);
    }

}
