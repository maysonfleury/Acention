using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    private GrapplingHook grappleHook;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

    void Start()
    {
        if (grappleHook == null)
            grappleHook = GetComponentInChildren<GrapplingHook>();
    }

    private void Update() {
        if(!grappleHook.isGrappling())
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grappleHook.GetGrapplePoint() - transform.position);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }
}
