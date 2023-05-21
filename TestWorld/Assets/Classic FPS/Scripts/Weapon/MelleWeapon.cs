using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class MelleWeapon : BaseWeaponScript
{
    [Line("Base Settings")]
    public AlternativeMelleType AlternativeMode;
    public float FOV = 60;
    public LayerMask enemyMask;
    public LayerMask occlusionMask;

    [Line("Base Attack")]
    public float weaponDamage = 30;
    public float impactForce = 20;
    public float range = 2.0f;
    public float angle = 90f;
    public float damageActivatorTime = 9f / 60;
    public float delay = .25f;
    public List<AnimationClip> normalAttackAnimation; 

    [Line("Alternative Attack")]
    [ShowIf("AlternativeMode", 1)] public float AlternativeWeaponDamage = 45;
    [ShowIf("AlternativeMode", 1)] public float AlternativeImpactForce = 25;
    [ShowIf("AlternativeMode", 1)] public float AlternativeRange = 2.0f;
    [ShowIf("AlternativeMode", 1)] public float AlternativeAngle = 145f;
    [ShowIf("AlternativeMode", 1)] public float AlternativeDamageActivatorTime = 9f / 60;
    [ShowIf("AlternativeMode", 1)] public float AlternativeDelay = .35f;
    [ShowIf("AlternativeMode", 1)] public List<AnimationClip> AlternativeAttackAnimation;

    [Line("Block")]
    [ShowIf("AlternativeMode", 2)] public float BlockingDamageChange = 30;
    [ShowIf("AlternativeMode", 2)] public float DamageReductionPercentage = 30;
    [ShowIf("AlternativeMode", 2)] public List<AnimationClip> BlockAnimation;

    [Line("UI")]
    public Image WeaponUIImage;
    public TMP_Text WeaponNameText;

    [Line("Sound")]
    public AudioClip attackSound;
    [ShowIf("AlternativeMode", 1)] public AudioClip attackAlternativeSound;

    [Line("Events")]
    public UnityEvent AdditionalBaseAttackFunction;
    public UnityEvent AdditionalAlternativeAttackFunction;
    public UnityEvent AdditionalBlockFunction;

    // Privates
    [HideInInspector] public Animator anim;
    AudioSource source;
    Transform mainCamera;

    // Field on View
    List<GameObject> baseAttackTargetObjects = new List<GameObject>();
    List<GameObject> alternativeAttackTargetObjects = new List<GameObject>();
    Collider[] baseColliders = new Collider[50];
    Collider[] alternativeColliders = new Collider[50];
    int baseCount;
    int alternativeCount;
    Mesh angleMesh;
    Mesh AlternativeAngleMesh;

    float timer;
    private TinyInput tinyInput;


    void Awake()
    {
        tinyInput = InputManager.Instance.input;
        // GetComponents
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        mainCamera = Camera.main.transform;

        WeaponNameText.text = weaponName.localization.GetString(weaponName.key);
        WeaponUIImage.sprite = weaponIcon;
        // Ready to Attack
        timer = 666;
        //tinyInput.Enable();

        // Base Attack
        tinyInput.Weapon.Fire.started += ctx => { if (timer > delay && !Application.isMobilePlatform) { StartCoroutine(BaseAttack()); timer = 0; } };

        // alternative Attack
        tinyInput.Weapon.Aim.started += ctx => { if (timer > AlternativeDelay && !Application.isMobilePlatform && AlternativeMode == AlternativeMelleType.AlternativeAttack) { StartCoroutine(AlternativeAttack()); timer = 0; } };
        // Block
        tinyInput.Weapon.Aim.started += ctx => { if (timer > delay && !Application.isMobilePlatform && AlternativeMode == AlternativeMelleType.Block) { anim.SetBool("Block", true); AdditionalBlockFunction.Invoke(); } };
        tinyInput.Weapon.Aim.canceled += ctx => { if (timer > delay && !Application.isMobilePlatform && AlternativeMode == AlternativeMelleType.Block) { anim.SetBool("Block", false); } };
    }
    public void OnDisable() {  }

    void Update()
    {
        if (!GameManager.Instance.paused)
        {
            timer += Time.deltaTime;

            mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, FOV, .5f);
        }

        // Object in Range
        baseCount = Physics.OverlapSphereNonAlloc(transform.position, range, baseColliders, enemyMask, QueryTriggerInteraction.Collide);
        alternativeCount = Physics.OverlapSphereNonAlloc(transform.position, AlternativeRange, alternativeColliders, enemyMask, QueryTriggerInteraction.Collide);
        
        baseAttackTargetObjects.Clear();
        alternativeAttackTargetObjects.Clear();

        for(int i = 0; i < baseCount; i++)
        {
            GameObject obj = baseColliders[i].gameObject;
            if (IsInSight(obj, angle))
            {
                baseAttackTargetObjects.Add(obj);
            }
        }
        for (int i = 0; i < alternativeCount; i++)
        {
            GameObject obj = alternativeColliders[i].gameObject;
            if (IsInSight(obj, AlternativeAngle))
            {
                alternativeAttackTargetObjects.Add(obj);
            }
        }
    }

    void FixedUpdate()
    {
      
    }

    IEnumerator BaseAttack()
    {
        anim.Play(normalAttackAnimation[Random.Range(0,normalAttackAnimation.Count)].name, -1, 0f);
        source.PlayOneShot(attackSound);
        anim.SetBool("Block", false);
        AdditionalBaseAttackFunction.Invoke();
        yield return new WaitForSeconds(damageActivatorTime);
        for(int i = 0; i < baseAttackTargetObjects.Count; i++)
        {
            baseAttackTargetObjects[i].SendMessage("TakeDamage", weaponDamage, SendMessageOptions.DontRequireReceiver);
            baseAttackTargetObjects[i].SendMessage("ExplosionBlood", SendMessageOptions.DontRequireReceiver);
        }
    }

    IEnumerator AlternativeAttack()
    {
        anim.Play(AlternativeAttackAnimation[Random.Range(0, AlternativeAttackAnimation.Count)].name, -1, 0f);
        source.PlayOneShot(attackAlternativeSound);
        anim.SetBool("Block", false);
        AdditionalAlternativeAttackFunction.Invoke();
        yield return new WaitForSeconds(AlternativeDamageActivatorTime);
        for (int i = 0; i < alternativeAttackTargetObjects.Count; i++)
        {
            alternativeAttackTargetObjects[i].SendMessage("TakeDamage", AlternativeWeaponDamage, SendMessageOptions.DontRequireReceiver);
            alternativeAttackTargetObjects[i].SendMessage("ExplosionBlood", SendMessageOptions.DontRequireReceiver);
        }
    }

    public override void PrimaryModeFunction()
    {
        //
    }

    public override void SecondaryModeFunction()
    {
        //
    }

    public override void ReloadFunction()
    {
        // Do Nothing
    }

    Mesh CreateWedgeMesh(float angle, float range)
    {
        Mesh mesh = new Mesh();

        int segments = 16;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] verticles = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.down;
        Vector3 bottomLeft = Vector3.down + Quaternion.Euler(0, -angle, 0) * Vector3.forward * range;
        Vector3 bottomRight = Vector3.down + Quaternion.Euler(0, angle, 0) * Vector3.forward * range;

        Vector3 topCenter = bottomCenter + Vector3.up * 2;
        Vector3 topLeft = bottomLeft + Vector3.up * 2;
        Vector3 topRight = bottomRight + Vector3.up * 2;

        int vert = 0;

        // left side
        verticles[vert++] = bottomCenter;
        verticles[vert++] = bottomLeft;
        verticles[vert++] = topLeft;

        verticles[vert++] = topLeft;
        verticles[vert++] = topCenter;
        verticles[vert++] = bottomCenter;

        // right side
        verticles[vert++] = bottomCenter;
        verticles[vert++] = topCenter;
        verticles[vert++] = topRight;

        verticles[vert++] = topRight;
        verticles[vert++] = bottomRight;
        verticles[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for(int i = 0; i <segments; i++)
        {
            bottomLeft = Vector3.down + Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * range;
            bottomRight = Vector3.down + Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * range;

            topLeft = bottomLeft + Vector3.up * 2;
            topRight = bottomRight + Vector3.up * 2;

            // far side
            verticles[vert++] = bottomLeft;
            verticles[vert++] = bottomRight;
            verticles[vert++] = topRight;

            verticles[vert++] = topRight;
            verticles[vert++] = topLeft;
            verticles[vert++] = bottomLeft;

            // top
            verticles[vert++] = topCenter;
            verticles[vert++] = topLeft;
            verticles[vert++] = topRight;

            // bottom
            verticles[vert++] = bottomCenter;
            verticles[vert++] = bottomRight;
            verticles[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for(int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = verticles;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        angleMesh = CreateWedgeMesh(angle, range);
        AlternativeAngleMesh = CreateWedgeMesh(AlternativeAngle, AlternativeRange);
    }

    private void OnDrawGizmos()
    {
        if (angleMesh)
        {
            Gizmos.color = new Color(1, .75f, 0, .5f);
            Gizmos.DrawMesh(angleMesh, transform.position, transform.rotation);

            Gizmos.DrawWireSphere(transform.position, range);
        }

        if (AlternativeAngleMesh && AlternativeMode == AlternativeMelleType.AlternativeAttack)
        {
            Gizmos.color = new Color(0, 1, 1, .2f);
            Gizmos.DrawMesh(AlternativeAngleMesh, transform.position, transform.rotation);

            Gizmos.DrawWireSphere(transform.position, AlternativeRange);
        }

        Gizmos.color = new Color(1, 0, 0, .2f);
        for (int i = 0; i < baseCount; i++)
        {
            Gizmos.DrawSphere(baseColliders[i].transform.position, 0.2f);
        }
        for (int i = 0; i < alternativeCount; i++)
        {
            Gizmos.DrawCube(alternativeColliders[i].transform.position, new Vector3(.15f, .15f, .15f));
        }

        Gizmos.color = new Color(0, 1, 0, .2f);
        foreach(var obj in baseAttackTargetObjects)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
        foreach (var obj in alternativeAttackTargetObjects)
        {
            Gizmos.DrawCube(obj.transform.position, new Vector3(.15f, .15f, .15f));
        }
    }

    public bool IsInSight(GameObject obj, float angle)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if(direction.y < -3 || direction.y > 3)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if(deltaAngle > angle)
        {
            return false;
        }

        //origin.y += 
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, occlusionMask))
        {
            return false;
        }

        return true;
    }
}
