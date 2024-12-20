using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private WaypointPath waypointPath;
    [SerializeField] private float speed;

    private Rigidbody rigidBody;

    private int targetWaypointIndex;
    private Transform previousWaypoint;
    private Transform targetWaypoint;

    private float timeToWaypoint;
    private float elapsedTime;

    void Start()
    {
        TargetNextWaypoint();
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        float elapsedPercentage = elapsedTime / timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage); // Slows down platform when nearing a waypoint
        Vector3 movePos = Vector3.Lerp(previousWaypoint.position, targetWaypoint.position, elapsedPercentage);
        Quaternion moveRot = Quaternion.Lerp(previousWaypoint.rotation, targetWaypoint.rotation, elapsedPercentage);

        rigidBody.MovePosition(movePos);
        rigidBody.MoveRotation(moveRot);

        if (elapsedPercentage >= 1)
            TargetNextWaypoint();
    }

    private void TargetNextWaypoint()
    {
        previousWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);
        targetWaypointIndex = waypointPath.GetNextWaypointIndex(targetWaypointIndex);
        targetWaypoint = waypointPath.GetWaypoint(targetWaypointIndex);

        elapsedTime = 0f;

        float distanceToWaypoint = Vector3.Distance(previousWaypoint.position, targetWaypoint.position);
        timeToWaypoint = distanceToWaypoint / speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<RigidBodyMovement>().moving = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<RigidBodyMovement>().moving = false;
        }
    }
}