using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public float rotationSpeed;
    public float motorForce;
    float rotation;
    HingeJoint2D joint;
    public GameObject spawnedBreakable;
    public GameObject spawnPoint;
    public float blockSpawnTime = 10f;

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        InvokeRepeating("SpawnOnObject", 0f, blockSpawnTime);
    }

    private void Update()
    {
        rotation = -1;
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
        Instantiate(spawnedBreakable, spawnPoint.transform.position, Quaternion.identity);
    }
}

