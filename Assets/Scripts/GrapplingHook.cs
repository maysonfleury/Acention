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
    public GameObject projectilePrefab;
    public float maxGrappleDistance = 100f;
    public int shotsLeft = 3;

    private GameObject tempProjectile;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0))
        {
            if(shotsLeft < 1)
            {
                particleShot.Play();
                // Play no shot SFX
                canShoot = false;
            }
            if(canShoot)
            {
                StartGrapple();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
        /*else if (Input.GetButton("Jump") && isGrappling())
        {
            RetractGrapple();
        }
        else if(Input.GetButtonUp("Jump") && isGrappling())
        {
            StopRetracting();
        }*/
        else if (Input.GetMouseButtonDown(1) && isGrappling())
        {
            RetractGrapple();
        }
        else if(Input.GetMouseButtonUp(1) && isGrappling())
        {
            StopRetracting();
        }

        if(rbMove.isGrounded())
        {
            canShoot = true;
            shotsLeft = 3;
        }
    }

    private void LateUpdate() {
        DrawLineRenderer();
    }

    private void StartGrapple()
    {
        // If the Raycast misses a target
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

            // How the rope feels
            // Test and change
            springJoint.spring = 4.5f;
            springJoint.damper = 5f;
            springJoint.massScale = 4.5f;

            // Add vertices to line renderer
            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
        else
        {
            // If the target missed we don't need to make a spring component

            // Show Particle Effect
            particleShot.Play();
            // Play SFX

            tempProjectile = Instantiate(projectilePrefab, gunTip.position, gunTip.rotation);
            tempProjectile.GetComponent<Rigidbody>().AddForce(gunTip.forward * 500f);
            Destroy(tempProjectile, 1.5f);

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }

        // No grappling more than once per airtime
        //canShoot = false;

        // Reduce shots by 1
        shotsLeft--;
    }

    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(springJoint);
    }

    private void RetractGrapple()
    {
        springJoint.maxDistance = 5f;
        springJoint.minDistance = 0.25f;
        //springJoint.spring = 10f;
        Debug.Log("Retract");
    }

    private void StopRetracting()
    {
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
        springJoint.maxDistance = distanceFromPoint * 0.9f;
        springJoint.minDistance = distanceFromPoint * 0.65f;
        springJoint.spring = 4.5f;
    }

    private void DrawLineRenderer()
    {
        // Don't draw if we're not grappled
        //if(!springJoint) return;
        if(lr.positionCount == 0) return;

        if(tempProjectile)
        {
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, tempProjectile.transform.position, Time.deltaTime * 8f);
        }
        else if(!tempProjectile && isGrappling())
        {
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        }
        else
        {
            lr.positionCount = 0;
        }

        if(lr.positionCount == 0) return;

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
