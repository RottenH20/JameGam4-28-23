using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public float rotationSpeed;
    public float motorForce;
    float rotation;
    HingeJoint2D joint;
    public GameObject sBLeft, sBRight;
    public GameObject spawnPointLeft;
    public GameObject spawnPointRight;
    public float blockSpawnTime = 1.5f;

    Transform hammerParent;

    public enum Direction
    {
        Left,
        Right
    }

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        InvokeRepeating("SpawnOnObject", 0f, blockSpawnTime);
        hammerParent = transform.GetChild(0);
    }

    private void Update()
    {
        rotation = -1;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            for (int i = 0; i < hammerParent.childCount; i++)
            {
                hammerParent.GetChild(i).gameObject.SetActive(i + 1 == 1);
            }
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            for (int i = 0; i < hammerParent.childCount; i++)
            {
                hammerParent.GetChild(i).gameObject.SetActive(i + 1 == 2);
            }
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            for (int i = 0; i < hammerParent.childCount; i++)
            {
                hammerParent.GetChild(i).gameObject.SetActive(i + 1 == 3);
            }
        }
    }

    private Direction GetRandomDirection()
    {
        int randomIndex = Random.Range(0, System.Enum.GetNames(typeof(Direction)).Length);
        return (Direction)randomIndex;
    }

    private void FixedUpdate()
    {
        joint.motor = new JointMotor2D
        {
            motorSpeed = rotationSpeed * rotation,
            maxMotorTorque = motorForce,
        };
    }

    void SpawnOnObject()
    {
        Direction tmp = GetRandomDirection();

        switch (tmp)
        {
            case Direction.Left :
                Instantiate(sBRight, spawnPointLeft.transform.position, Quaternion.identity);
                break;
            default :
                Instantiate(sBLeft, spawnPointRight.transform.position, Quaternion.identity);
                break;
        }
    }
}

