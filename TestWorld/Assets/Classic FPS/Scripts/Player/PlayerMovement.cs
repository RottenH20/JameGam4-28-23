/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.1.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    public float playerWalkingSpeed = 5f;
    public float playerRunningSpeed = 15f;
    public float jumpStrength = 20f;
    public float verticalRotationLimit = 80f;
    public AudioClip pickupSound;
    public FlashScreen flash;
    //[Line("Interaction")]
    //public float interactionDistance = 3f;
    //public LayerMask interactionMask;
    [Line  ("Mobile")]
    public MobileJoystick MovementJoystick;
    public MobileJoystick CameraJoystick;

    float forwardMovement;
    float sidewaysMovement;
    float verticalVelocity;
    float verticalRotation = 0;
    float horizontalRotation = 0;

    GameObject PopupScreen;
    private TinyInput tinyInput;
    CharacterController cc;
    AudioSource source;
    GameSettingsManger _gm;

    void Awake()
    {
        tinyInput = InputManager.Instance.input;
        var rebinds = PlayerPrefs.GetString("rebinds");
        tinyInput.asset.LoadBindingOverridesFromJson(rebinds);

        cc = GetComponent<CharacterController>();
        source = GetComponent<AudioSource>();
        PopupScreen = transform.Find("Main Panels").Find("PickUp Panel").gameObject;

        _gm = (GameSettingsManger)Resources.Load("Game_Settings");
        if (_gm.EnablePlayerAbility)
        {
            PassiveAbilitySO _passive = GameManager.Instance.PassiveSkill;
            if (_passive.BoostSpeed)
            {
                playerRunningSpeed *= _passive.SpeedIncrease;
                playerWalkingSpeed *= _passive.SpeedIncrease;
            }
        }
    }
    public void OnEnable() { tinyInput.Enable(); }
    public void OnDisable(){ tinyInput.Disable(); }

    public Vector2 MovementValue()
    {
        if (MovementJoystick.Direction.x != 0 && MovementJoystick.Direction.y != 0) return MovementJoystick.Direction;
        return tinyInput.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 LookValue()
    {
        if (CameraJoystick.Direction.x != 0 && CameraJoystick.Direction.y != 0) return CameraJoystick.Direction * 8;
        return tinyInput.Player.Look.ReadValue<Vector2>();
    }

    void Update()
    {
        if (!GameManager.Instance.paused)
        {
            horizontalRotation = LookValue().x;
            verticalRotation -= LookValue().y;
            
            // Look from side to side
            transform.Rotate(0, horizontalRotation, 0);
            // Look up and down
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
            Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            // Move the player
            // Only if in contact with the ground
            if (cc.isGrounded)
            {

                    forwardMovement = MovementValue().y * playerWalkingSpeed;
                    sidewaysMovement = MovementValue().x * playerWalkingSpeed;
                    // Run if the player pressed the Left Shift
                    if (tinyInput.Player.Sprint.ReadValue<float>() == 1 && _gm.EnablePlayerSprint)
                    {
                        forwardMovement = MovementValue().y * playerRunningSpeed;
                        sidewaysMovement = MovementValue().x * playerRunningSpeed;
                    }
                
                if (tinyInput.Player.Move.ReadValue<Vector2>().y != 0 || tinyInput.Player.Move.ReadValue<Vector2>().x != 0)
                {
                    if (tinyInput.Player.Sprint.ReadValue<float>() == 1 && _gm.EnablePlayerSprint)
                    {
                        DynamicCrosshair.spread = DynamicCrosshair.RUN_SPREAD;
                    }
                    else
                    {
                        DynamicCrosshair.spread = DynamicCrosshair.WALK_SPREAD;
                    }
                }
            }
            else
            {
                DynamicCrosshair.spread = DynamicCrosshair.JUMP_SPREAD;
            }
            // Make the player act with gravity
            // So, let the substrate attract him
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

            // Jumping after pressing the jump button
            if (tinyInput.Player.Jump.ReadValue<float>() == 1)
            {
                UIJump();
            }
            if (tinyInput.Player.Interaction.ReadValue<float>() == 1)
            {
                //UiInteraction();
            }

            Vector3 playerMovement = new Vector3(sidewaysMovement, verticalVelocity, forwardMovement);
            // Moving the hero
            cc.Move(transform.rotation * playerMovement * Time.deltaTime);
        }

    }

    public void UIJump()
    {
        if (cc.isGrounded) { verticalVelocity = jumpStrength; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthBonus"))
        {
            BonusScript _bs = other.transform.GetComponent<BonusScript>();
            GetComponent<PlayerHealth>().AddHealth(_bs.Health, _bs.godBonus);

            flash.HealthBonus();
            source.PlayOneShot(pickupSound);

        }

        else if (other.CompareTag("ArmorBonus"))
        {
            BonusScript _bs = other.transform.GetComponent<BonusScript>();
            GetComponent<PlayerHealth>().AddArmor(_bs.Armor, _bs.godBonus);

            flash.ArmorBonus();
            source.PlayOneShot(pickupSound);
        }

        else if (other.CompareTag("AmmoBonus"))
        {
            BonusScript _bs = other.transform.GetComponent<BonusScript>();
            GameManager.Instance.AddAmmo(_bs.ammoType, _bs.AmmoAmount);

            WeaponSwitch container = GetComponentInChildren(typeof(WeaponSwitch)) as WeaponSwitch;
            if (container != null) { container.actualWeapon.UpdateLeftAmmo(); }

            flash.AmmoBonus();
            source.PlayOneShot(pickupSound);
        }

        else if (other.CompareTag("KeyBonus"))
        {
            switch (other.GetComponent<BonusScript>().keyType)
            {
                case (KeyType.RedKey):
                    LevelManager.Instance.redKeys = true;
                    break;
                case (KeyType.BlueKey):
                    LevelManager.Instance.blueKeys = true;
                    break;
                case (KeyType.YellowKey):
                    LevelManager.Instance.yellowKeys = true;
                    break;
                case (KeyType.GreenKey):
                    LevelManager.Instance.greenKeys = true;
                    break;
                default:
                    break;
            }
            flash.AmmoBonus();
            source.PlayOneShot(pickupSound);
        }

        if (other.CompareTag("HealthBonus") || other.CompareTag("ArmorBonus") || other.CompareTag("AmmoBonus") || other.CompareTag("KeyBonus"))
        {
            StopCoroutine("SetupPanel");
            StartCoroutine(SetupPanel(other));
            Destroy(other.gameObject);
        }
    }

    public IEnumerator SetupPanel(Collider other)
    {
        BonusScript bs = other.GetComponent<BonusScript>();

        if (bs.bonusType == BonusType.Key)
        {
            PopupScreen.transform.Find("Description").GetComponent<TMPro.TMP_Text>().text = "";
            PopupScreen.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key);
            StartCoroutine(GameManager.Instance.SpawnInGamePopup(bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key)));
        }
        if (bs.bonusType == BonusType.Health)
        {
            PopupScreen.transform.Find("Description").GetComponent<TMPro.TMP_Text>().text = "<color=#1EBE0A>+" + bs.Health + " " + bs.PickUpLocalization.localization.GetString("Health") + "</color>";
            PopupScreen.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key);
            StartCoroutine(GameManager.Instance.SpawnInGamePopup(bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key)));
        }
        if (bs.bonusType == BonusType.Armor)
        {
            PopupScreen.transform.Find("Description").GetComponent<TMPro.TMP_Text>().text = "<color=#008AFF>+" + bs.Armor + " " + bs.PickUpLocalization.localization.GetString("Armor") + "</color>";
            PopupScreen.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key);
            StartCoroutine(GameManager.Instance.SpawnInGamePopup(bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key)));
        }
        if (bs.bonusType == BonusType.Ammo)
        {
            PopupScreen.transform.Find("Description").GetComponent<TMPro.TMP_Text>().text = "<color=#E59B10>+" + bs.AmmoAmount + " " + bs.PickUpLocalization.localization.GetString(bs.ammoType.ToString()) + "</color>";
            PopupScreen.transform.Find("Title").GetComponent<TMPro.TMP_Text>().text = bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key);
            StartCoroutine(GameManager.Instance.SpawnInGamePopup(bs.PickUpLocalization.localization.GetString(bs.PickUpLocalization.key) + " " + bs.BonusLocalization.localization.GetString(bs.BonusLocalization.key)));
        }

        PopupScreen.GetComponent<CanvasGroup>().alpha = 1;
        yield return new WaitForSeconds(2);
        PopupScreen.GetComponent<CanvasGroup>().alpha = 0;
    }
}
