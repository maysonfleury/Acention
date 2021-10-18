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
    public ParticleSystem particleShot;
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
        else if(Input.GetButtonUp("Jump") && isGrappling())
        {
            StopRetracting();
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
        // If the Raycast hits a target
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            // Show Particle Effect
            particleShot.Play();

            // Initialize Spring Component
            grapplePoint = hit.point;
            springJoint = player.gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            // Distance the grapple will try to keep from grapple point
            springJoint.maxDistance = distanceFromPoint * 0.9f;
            springJoint.minDistance = distanceFromPoint * 0.75f;

            // Test and change
            springJoint.spring = 4.5f;
            springJoint.damper = 5f;
            springJoint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
        else
        {
            // If the target missed
        }

        // No grappling more than once per airtime
        //canShoot = false;
    }

    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(springJoint);
    }

    private void RetractGrapple()
    {
        springJoint.maxDistance = 0.25f;
        springJoint.minDistance = 0.25f;
    }

    private void StopRetracting()
    {
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
        springJoint.maxDistance = distanceFromPoint * 0.9f;
        springJoint.minDistance = distanceFromPoint * 0.9f;
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
