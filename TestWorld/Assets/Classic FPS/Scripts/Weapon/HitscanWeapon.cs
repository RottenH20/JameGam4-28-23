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
public class HitscanWeapon : BaseWeaponScript
{
    [Line(VirtualColor.Red, "AS OF VERSION 1.4.0, THIS SCRIPT IS DEPRECATED","PLEASE USE ,,RANGEDWEAPON.CS''")]
    //[Header("Weapon Settings")]
    public PrimaryFireMode PrimaryMode;
    public SecondaryFireMode SecondaryMode;

    [Header("Normal Mode")]
    public float weaponSpread;
    public float baseFOV;
    [Header("Aim Mode")]
    public float weaponAimSpread;
    public float aimFOV;

    [Header("Ammo & Damage")]
    public float weaponDamage;
    public float weaponRange;
    public float timeBetweenShot;
    public int magazineSize;
    public AmmoType ammoType;
    public LayerMask raycastMask;

    [Header("Sound")]
    public AudioClip shotSound;
    public AudioClip reloadSound;
    public AudioClip emptyGunSound;

    [Header("Ui")]
    public Sprite UISprite;
    public TMP_Text ammoText;
    public Image WeaponUIImage;

    [Header("Object & Prefabs")]
    public GameObject bulletHole;

    // Privates
    Transform mainCamera;
    int ammoLeft;
    int ammoClipLeft = 666;
    bool isShot;
    bool isReloading;
    bool aim = false;
    float timer;
    float currentSpread;
    AudioSource source;
    Animator anim;

    void Awake()
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
        //muzzleflashLight.SetActive(false);
        WeaponUIImage.sprite = UISprite;
        timer = 666;
    }

    void Update()
    {
        if (!GameManager.Instance.paused)
        {
            if (ammoType != AmmoType.InfinityAmmo && magazineSize < 1) ammoText.text = ammoLeft.ToString();
            else if (ammoType != AmmoType.InfinityAmmo) ammoText.text = ammoClipLeft + "<size=8> / " + ammoLeft;
            else if (ammoType == AmmoType.InfinityAmmo && magazineSize > 0) ammoText.text = ammoClipLeft + "<size=8> / Infinity";
            else ammoText.text = "Infinity";

            timer += Time.deltaTime;

            if (PrimaryMode == PrimaryFireMode.Single && Input.GetKeyDown(KeyCode.Mouse0) && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft > 0) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } }
            if (PrimaryMode == PrimaryFireMode.Automatic && Input.GetKey(KeyCode.Mouse0) && isReloading == false && timer > timeBetweenShot && !Application.isMobilePlatform) { if (magazineSize <= 0) { if (ammoLeft > 0) { isShot = true; } else { source.PlayOneShot(emptyGunSound); timer = 0; } } else { isShot = true; } }
            if (Input.GetKeyDown(KeyCode.R) && isReloading == false && ammoClipLeft != magazineSize) { Reload(); }

            if (!Application.isMobilePlatform)
            {
                if (Input.GetKey(KeyCode.Mouse1) && SecondaryMode == SecondaryFireMode.Aim && isReloading == false) { anim.SetBool("aim", true); aim = true; } else { anim.SetBool("aim", false); aim = false; }
            }

            // Change FOV
            if (aim) { mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, aimFOV, .5f); currentSpread = weaponAimSpread; }
            else { mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, baseFOV, .5f); currentSpread = weaponSpread; }
        }

    }

    void FixedUpdate()
    {
        // Calculate a random shift within a circle
        // The radius of the circle depends on the current value of the variable 'spread'
        Vector2 bulletOffset = Random.insideUnitCircle * DynamicCrosshair.spread;

        // We create a ray that goes from our camera to the center of the screen with the shift
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward * weaponRange);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, weaponRange, raycastMask))
        {
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * weaponRange, new Color(0, 1, 0, 1));
        }

        if (isShot == true && ammoClipLeft > 0 && isReloading == false)
        {
            isShot = false;
            DynamicCrosshair.spread += currentSpread;

            if (magazineSize >= 1) ammoClipLeft--;
            else if (ammoType != AmmoType.InfinityAmmo) { GameManager.Instance.AddAmmo(ammoType, -1); ammoLeft--; }

            source.PlayOneShot(shotSound);
            StartCoroutine("shot");
            // If after pressing the 'Fire1' button the beam collides with an object
            // Runs the following instructions
            if (Physics.Raycast(ray, out hit, weaponRange, raycastMask))
            {
                // Send information to the hit object that we hit it
                // The hit object should fire the AddDamage function with the weaponDamage parameter
                hit.collider.gameObject.SendMessage("TakeDamage", weaponDamage, SendMessageOptions.DontRequireReceiver);
                if (hit.transform.CompareTag("Enemy"))
                {
                    Enemy _enemy = hit.transform.GetComponent<Enemy>();
                    Instantiate(_enemy.Blood[Random.Range(0, _enemy.Blood.Length)], hit.point, Quaternion.identity);
                    if (hit.collider.gameObject.GetComponent<EnemyStates>().currentState == hit.collider.gameObject.GetComponent<EnemyStates>().patrolState ||
                        hit.collider.gameObject.GetComponent<EnemyStates>().currentState == hit.collider.gameObject.GetComponent<EnemyStates>().alertState)
                        // Send the target firing position to the target if it is in a patrol or alert state
                        hit.collider.gameObject.SendMessage("HiddenShot", transform.parent.transform.position, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    // We Create a Bullet Hole Object on the Object We Hit with the Ray
                    // By the way, we change the parent of the bullet hole to become a sub-object of the target object
                    Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)).transform.parent =
                        hit.collider.gameObject.transform;
                }
            }
            timer = 0;
        }
        else if (isShot == true && ammoClipLeft <= 0 && isReloading == false)
        {
            // When we shoot but have no ammo left, reload the gun
            isShot = false;
            Reload();
        }
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
    IEnumerator ReloadWeapon()
    {
        isReloading = true;
        source.PlayOneShot(reloadSound);
        anim.Play("Reload", -1, 0f);
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

    public void AddAmmo(int value)
    {
        ammoLeft += value;
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