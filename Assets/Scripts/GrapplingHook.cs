using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private SpringJoint springJoint;
    private Vector3 currentGrapplePosition;
    private bool canShoot;

    public RigidBodyMovement rbMove;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public float maxGrappleDistance = 100f;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0))
        {
            if(canShoot)
            {
                StartGrapple();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
        else if (Input.GetButton("Jump") && isGrappling())
        {
            RetractGrapple();
        }

        if(rbMove.isGrounded())
        {
            canShoot = true;
        }
    }

    private void LateUpdate() {
        DrawChain();
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            springJoint = player.gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            // Distance the grapple will try to keep from grapple point
            springJoint.maxDistance = distanceFromPoint * 0.8f;
            springJoint.minDistance = distanceFromPoint * 0.8f;

            // Test and change
            springJoint.spring = 4.5f;
            springJoint.damper = 7f;
            springJoint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
        canShoot = false;
    }

    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(springJoint);
    }

    private void RetractGrapple()
    {
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
        springJoint.maxDistance = 0.25f;
        springJoint.minDistance = 0.25f;
    }

    private void DrawChain()
    {
        // Don't draw chain if we're not grappled
        if(!springJoint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool isGrappling()
    {
        return (springJoint != null);
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
