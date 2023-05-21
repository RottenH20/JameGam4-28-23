using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    public GameObject platform;
    public List<GameObject> Waypoints;
    public float speed;
    public float stayTime = 1;

    // Privates
    int waypointIndex;
    Vector3 velocity = Vector3.zero;
    float timeLeft;

    public void Update()
    {
        // Movement
        if(timeLeft < 0) platform.transform.position = Vector3.SmoothDamp(platform.transform.position, Waypoints[waypointIndex].transform.position, ref velocity, speed * Time.deltaTime, speed);
        // Get Next Target
        timeLeft -= Time.deltaTime;
        if (.05f > Vector3.Distance(platform.transform.position, Waypoints[waypointIndex].transform.position)) { waypointIndex++;  if (waypointIndex >= Waypoints.Count) { waypointIndex = 0; } timeLeft = stayTime; }
    }
}
