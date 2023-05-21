using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class RangeWeapon : BaseWeaponScript
{
    [Line("Base Settings")]
    public AnimationWeaponType AnimationType;
    public WeaponTypeShooting WeaponShootType;
    public RangeWeaponShootType FireMode;
    public BulleType bulletType;

    [Line("Camera & Aiming")]
    public float baseFOV = 60;
    public bool enableAiming = false;
    [ShowIf("enableAiming")] public float aimFOV = 45;

    [Line("Damage & Ammo")]
    [HideIf("FireMode", (int)RangeWeaponShootType.Laser)] public int ammoCostPerShot = 1;
    [HideIf("FireMode", (int)RangeWeaponShootType.Laser)] public int bulletCountPerShot = 1;
    [ShowIf("FireMode", (int)RangeWeaponShootType.Laser)] public float timeBetweenLaserAmmo = .7f;
    [ShowIf("FireMode", (int)RangeWeaponShootType.Burst)] public int burstShot = 3;
    [ShowIf("FireMode", (int)RangeWeaponShootType.Charge)] public float ChargeTime = 1.2f;
    public float weaponDamage = 30;
    public float weaponSpread = 15;
    [ShowIf("enableAiming")] public float weaponSpreadWhenAiming = 12;
    [ShowIf("WeaponShootType", (int)WeaponTypeShooting.Projectile)] public float bulletForce = 50;
    [ShowIf("WeaponShootType", (int)WeaponTypeShooting.Hitscan)] public float hitscanRange = 50;
    [ShowIf("bulletType", (int)BulleType.Explosion)] public float explosionRadius = 3.5f;
    [ShowIf("bulletType", (int)BulleType.Explosion)] public AnimationCurve explosionDamageCurve;
    [ShowIf("bulletType", (int)BulleType.Explosion)] public float explosionShakeDistance = 7;
    [Suffix("Secound")] public float timeBetweenShot = 0.15f;
    public int magazineSize = 12;
    public AmmoType ammoType;
    public LayerMask raycastMask;

    [Line("UI")]
    public TMP_Text ammoText;
    public TMP_Text WeaponNameText;
    public Image WeaponUIImage;

    [Line("Sound")]
    public AudioClip shotSound;
    public AudioClip reloadSound;
    public AudioClip emptyGunSound;
    [ShowIf("bulletType", (int)BulleType.Explosion)] public AudioClip explosionSound;

    [Line("Prefabs & Objects")]
    [ShowIf("bulletType", (int)BulleType.Normal)] public GameObject bulletHole;
    [ShowIf("WeaponShootType", (int)WeaponTypeShooting.Projectile)] public GameObject rocketPrefab;
    [ShowIf("bulletType", (int)BulleType.Explosion)] public GameObject explosionPrefab;
    [ShowIf("WeaponShootType", (int)WeaponTypeShooting.Projectile)] public GameObject spawnpointPrefab;

    [Line("Events")]
    public UnityEvent AdditionalFireWeaponFunction;
    public UnityEvent AdditionalWeaponAimFunction;
    public UnityEvent AdditionalWeaponReloadFunction;

    // Privates
    AudioSource source;
    Animator anim;
    Transform mainCamera;

    int ammoLeft;
    int ammoClipLeft = 666;

    bool isShot;
    bool isReloading;
    bool aim = false;
    float timer;
    float currentSpread;


    public float _chargeTime;
    public float _timeToNextAmmoLaser;
    //public bool _startCharge;

    private TinyInput tinyInput;


    void Awake()
    {
        tinyInput = InputManager.Instance.input;

        // GetComponents
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        // If not Infinity Ammo
        if (ammoType != AmmoType.InfinityAmmo) ammoLeft = GameManager.Instance.AmmoLeft(ammoType);
        // If doesn't have Magazine
        if (magazineSize > 1) ammoClipLeft = magazineSize;

        if (FireMode == RangeWeaponShootType.Burst) { timeBetweenShot += timeBetweenShot * burstShot; }

        GameSettingsManger _go = (GameSettingsManger)Resources.Load("Game_Settings");
        if (_go.EnablePlayerAbility)
        {
            PassiveAbilitySO _passive = GameManager.Instance.PassiveSkill;
            if (_passive.BoostWeapon)
            {
                weaponDamage *= (int)_passive.DamageIncrease;
                weaponSpread /= (int)_passive.RecoilReduce;
                weaponSpreadWhenAiming /= (int)_passive.RecoilReduce;
                timeBetweenShot /= (int)_passive.FireRateIncrease;
            }
        }

        //anim.animatior
    }

    void OnEnable()
    {
        isReloading = false;
        mainCamera = Camera.main.transform;

        // If not Infinity Ammo
        if (ammoType != AmmoType.InfinityAmmo) ammoLeft = GameManager.Instance.AmmoLeft(ammoType);

        WeaponNameText.text = weaponName.localization.GetString(weaponName.key);

        WeaponUIImage.sprite = weaponIcon;
        // Ready to Shot
        timer = 666;
        //tinyInput.Enable();


        // Single
        tinyInput.Weapon.Fire.started += ctx => { if (FireMode == RangeWeaponShootType.Single && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft >= ammoCostPerShot) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } } };

        // Burst
        tinyInput.Weapon.Fire.started += ctx => { if (FireMode == RangeWeaponShootType.Burst && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft >= ammoCostPerShot) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { StartCoroutine("BurstEnumerator"); } } };

        // Laser
        tinyInput.Weapon.Fire.started += ctx => { if (FireMode == RangeWeaponShootType.Laser && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform)
            {
                if (magazineSize <= 0) { if (ammoLeft >= ammoCostPerShot) { if (aim) { anim.Play("Start_Aim_Shot", -1, 0f); } else { anim.Play("Start_Shot", -1, 0f); } isShot = true; anim.SetBool("pressed", true); } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { if (aim) { anim.Play("Start_Aim_Shot", -1, 0f); } else { anim.Play("Start_Shot", -1, 0f); } isShot = true; anim.SetBool("pressed", true); }
            }
        };
        tinyInput.Weapon.Fire.canceled += ctx => { anim.SetBool("pressed", false); };


        
        // Reload
        tinyInput.Weapon.Reload.performed += ctx => { if (isReloading == false && ammoClipLeft != magazineSize) { Reload(); } };
    }
    //public void OnDisable() { tinyInput.Disable(); }


    void Update()
    {
        if (!GameManager.Instance.paused)
        {
            // Ammo Text
            if (ammoType != AmmoType.InfinityAmmo && magazineSize < 1) ammoText.text = ammoLeft.ToString();                             // No Magazine
            else if (ammoType != AmmoType.InfinityAmmo) ammoText.text = ammoClipLeft + "<size=8> / " + ammoLeft;                        // Normal
            else if (ammoType == AmmoType.InfinityAmmo && magazineSize > 0) ammoText.text = ammoClipLeft + "<size=8> / Infinity";       // Infinity Ammo with magazine
            else ammoText.text = "Infinity";                                                                                            // Infinity Ammo without magazine

            timer += Time.deltaTime;

            // Charge
            if (FireMode == RangeWeaponShootType.Charge && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform)
            {
                if (tinyInput.Weapon.Fire.ReadValue<float>() == 1 && timer > timeBetweenShot)
                {
                    
                    _chargeTime += Time.deltaTime;
                    if (_chargeTime > ChargeTime)
                    {
                        _chargeTime = 0;
                        if (magazineSize <= 0)
                        {
                            if (ammoLeft >= ammoCostPerShot) { isShot = true;  }
                            else { source.PlayOneShot(emptyGunSound); timer = 0; }
                        }
                        else { isShot = true; }
                    }
                }
            }

            // Automatic
            if (tinyInput.Weapon.Fire.ReadValue<float>() == 1) { if (FireMode == RangeWeaponShootType.Automatic && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft >= ammoCostPerShot) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } } }

            // Laser
            if (tinyInput.Weapon.Fire.ReadValue<float>() == 1) { if (FireMode == RangeWeaponShootType.Laser && isReloading == false && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft >= 1) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } } }

            // Aim on PC
            if (tinyInput.Weapon.Aim.ReadValue<float>() == 1 && enableAiming && isReloading == false) { anim.SetBool("aim", true); aim = true; AdditionalWeaponAimFunction.Invoke(); } else { anim.SetBool("aim", false); aim = false; }

            // Change FOV
            if (aim) { mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, aimFOV, .5f); mainCamera.Find("GUI_Camera").GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, aimFOV, .5f); currentSpread = weaponSpreadWhenAiming; }
            else { mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, baseFOV, .5f); mainCamera.Find("GUI_Camera").GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, baseFOV, .5f); currentSpread = weaponSpread; }
        }

    }

    void FixedUpdate()
    {
        if (isShot == true && ammoClipLeft > 0 && isReloading == false)
        {
            AdditionalFireWeaponFunction.Invoke();
            isShot = false;
            source.PlayOneShot(shotSound);
            StartCoroutine("shot");

            if (FireMode != RangeWeaponShootType.Laser)
            {
                if (magazineSize >= 1) ammoClipLeft -= ammoCostPerShot;
                else if (ammoType != AmmoType.InfinityAmmo) { GameManager.Instance.AddAmmo(ammoType, -1); ammoLeft -= ammoCostPerShot; }
            }
            else
            {
                _timeToNextAmmoLaser += Time.deltaTime;
                if (_timeToNextAmmoLaser > timeBetweenLaserAmmo)
                {
                    _timeToNextAmmoLaser = 0;
                    if (magazineSize >= 1) ammoClipLeft -= 1;
                    else if (ammoType != AmmoType.InfinityAmmo) { GameManager.Instance.AddAmmo(ammoType, -1); ammoLeft -= 1; }
                }
            }

            DynamicCrosshair.spread += currentSpread;

            if (WeaponShootType == WeaponTypeShooting.Hitscan)
            {
                float damage = weaponDamage;
                if (FireMode == RangeWeaponShootType.Laser) { damage = weaponDamage * Time.deltaTime; }

                float bulletCount = bulletCountPerShot;
                if (FireMode == RangeWeaponShootType.Laser) { bulletCount = 1; }

                for (int i = 0; i < bulletCount; i++)
                {

                    Vector3 deviation3D = Random.insideUnitCircle * DynamicCrosshair.spread / 10;
                    Quaternion rot = Quaternion.LookRotation(Vector3.forward * hitscanRange + deviation3D);
                    Vector3 mainCameraForwardWithSpread = mainCamera.transform.rotation * rot * Vector3.forward * hitscanRange;

                    Ray ray = new Ray(mainCamera.transform.position, mainCameraForwardWithSpread);

                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, hitscanRange, raycastMask))
                    {
                        hit.collider.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                        if (hit.transform.CompareTag("Enemy"))
                        {
                            if (bulletType == BulleType.Normal)
                            {
                                Enemy _enemy = hit.transform.GetComponent<Enemy>();
                                Instantiate(_enemy.Blood[Random.Range(0, _enemy.Blood.Length)], hit.point, Quaternion.identity);
                                if (hit.collider.gameObject.GetComponent<EnemyStates>().currentState == hit.collider.gameObject.GetComponent<EnemyStates>().patrolState || hit.collider.gameObject.GetComponent<EnemyStates>().currentState == hit.collider.gameObject.GetComponent<EnemyStates>().alertState)
                                    // Send the target firing position to the target if it is in a patrol or alert state
                                    hit.collider.gameObject.SendMessage("HiddenShot", transform.parent.transform.position, SendMessageOptions.DontRequireReceiver);
                            }
                        }
                        else
                        {
                            if (FireMode != RangeWeaponShootType.Laser) { Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)).transform.parent = hit.collider.gameObject.transform; }

                            if (bulletType == BulleType.Explosion)
                            {
                                GameObject explosionInstantiated = (GameObject)Instantiate(explosionPrefab, hit.point, Quaternion.identity);
                                explosionInstantiated.GetComponent<Explosion>().explosionSound = explosionSound;

                                Collider[] damageColliders = Physics.OverlapSphere(hit.point, explosionRadius);
                                foreach (var hitCollider in damageColliders)
                                {
                                    float _distance = Vector3.Distance(hitCollider.transform.position, hit.point);
                                    hitCollider.SendMessage("TakeDamage", damage * explosionDamageCurve.Evaluate(_distance / explosionRadius), SendMessageOptions.DontRequireReceiver);
                                    hitCollider.SendMessage("ExplosionBlood", SendMessageOptions.DontRequireReceiver);
                                }

                                if (Vector3.Distance(this.transform.position, Camera.main.transform.position) < explosionShakeDistance) { Camera.main.GetComponent<CameraShake>().ShakeCamera(); }
                            }
                        }
                    }
                }
            }

            if (WeaponShootType == WeaponTypeShooting.Projectile)
            {
                float damage = weaponDamage;
                if (FireMode == RangeWeaponShootType.Laser) { damage = weaponDamage * Time.deltaTime; }

                float bulletCount = bulletCountPerShot;
                if (FireMode == RangeWeaponShootType.Laser) { bulletCount = 1; }

                for (int i = 0; i < bulletCount; i++)
                {
                    GameObject rocketInstantiated = (GameObject)Instantiate(rocketPrefab, spawnpointPrefab.transform.position, Quaternion.identity);
                    rocketInstantiated.GetComponent<BulletScript>().damage = damage;
                    rocketInstantiated.GetComponent<BulletScript>().damageCurve = explosionDamageCurve;
                    rocketInstantiated.GetComponent<BulletScript>().radius = explosionRadius;
                    rocketInstantiated.GetComponent<BulletScript>().explosionSound = explosionSound;
                    rocketInstantiated.GetComponent<BulletScript>().layerMask = raycastMask;
                    rocketInstantiated.GetComponent<BulletScript>().explosion = explosionPrefab;
                    rocketInstantiated.GetComponent<BulletScript>().shakeRadius = explosionShakeDistance;

                    var randomNumberX = Random.Range(-DynamicCrosshair.spread / 900, DynamicCrosshair.spread / 900);
                    var randomNumberY = Random.Range(-DynamicCrosshair.spread / 900, DynamicCrosshair.spread / 900);
                    var randomNumberZ = Random.Range(-DynamicCrosshair.spread / 900, DynamicCrosshair.spread / 900);

                    //rocketInstantiated.GetComponent<BulletScript>().transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);
                    Vector3 ve = new Vector3(randomNumberX, randomNumberY, randomNumberZ);

                    Rigidbody rocketRb = rocketInstantiated.GetComponent<Rigidbody>();

                    rocketRb.AddForce((Camera.main.transform.forward + ve) * bulletForce, ForceMode.Impulse);
                }
            }
            timer = 0;
        }
        // Hitscan & Projectile
        else if (isShot == true && ammoClipLeft <= 0 && isReloading == false)
        {
            isShot = false;
            Reload();
        }
    }

    IEnumerator BurstEnumerator()
    {
        for (int i = 0; i < burstShot; i++)
        {
            isShot = true;
            yield return new WaitForSeconds(timeBetweenShot / (burstShot + 1));
        }
        yield return new WaitForSeconds(0);
    }

    // Function responsible for reloading weapons
    void Reload()
    {
        // Calculate How Many Bullets Should We Reload
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

                    GameManager.Instance.AddAmmo(ammoType, -bulletsToReload);
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

    IEnumerator ReloadWeapon()
    {
        isReloading = true;
        source.PlayOneShot(reloadSound);
        anim.Play("Reload", -1, 0f);

        AdditionalWeaponReloadFunction.Invoke();

        yield return new WaitForSeconds(.01f);
        float _reloadTime = anim.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(_reloadTime);
        isReloading = false;

        if (tinyInput.Weapon.Fire.ReadValue<float>() == 1 && FireMode == RangeWeaponShootType.Laser) { anim.Play("Start_Shot", -1, 0f); }
    }

    IEnumerator shot()
    {
        // When Aimong Set Aiming Shot Animation
        if (tinyInput.Weapon.Aim.ReadValue<float>() == 1 && enableAiming)
        {
            if (FireMode != RangeWeaponShootType.Laser) 
            {
                if (AnimationType == AnimationWeaponType.DualWeapon)
                {
                    if(ammoClipLeft % 2 == 0) { anim.Play("Aim_Shot_Right", -1, 0f); }
                    else { anim.Play("Aim_Shot_Left", -1, 0f); }
                }
                else { anim.Play("Aim_Shot", -1, 0f); }
            }
        }
        // Else Show Normal Shot Animation
        else 
        {
            if (FireMode != RangeWeaponShootType.Laser) 
            {
                if (AnimationType == AnimationWeaponType.DualWeapon)
                {
                    if (ammoClipLeft % 2 == 0) { anim.Play("Shot_Right", -1, 0f); }
                    else { anim.Play("Shot_Left", -1, 0f); }
                }
                else { anim.Play("Shot", -1, 0f); }
            }
        }

        yield return new WaitForSeconds(0.1f);
    }

    public void AddAmmo(int value) { ammoLeft += value; }

    /* Interactions */
    // Shoot
    public override void UpdateLeftAmmo()
    {
        if (ammoType != AmmoType.InfinityAmmo && magazineSize > 1) ammoLeft = GameManager.Instance.AmmoLeft(ammoType);
        else if (ammoType != AmmoType.InfinityAmmo && magazineSize <= 0) { ammoLeft = GameManager.Instance.AmmoLeft(ammoType); }
    }

    public override void PrimaryModeFunction()
    {
        if (isReloading == false && timer > timeBetweenShot) { if (magazineSize <= 0) { if (ammoLeft >= ammoCostPerShot) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } }
    }
    // Aim
    public override void SecondaryModeFunction()
    {
        if (enableAiming && isReloading == false) { anim.SetBool("aim", true); aim = true; AdditionalWeaponAimFunction.Invoke(); } else { anim.SetBool("aim", false); aim = false; }
    }
    // Reload
    public override void ReloadFunction()
    {
        if (isReloading == false && ammoClipLeft != magazineSize) { Reload(); }
    }

}