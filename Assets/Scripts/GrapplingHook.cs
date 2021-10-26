using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingHook : MonoBehaviour
{
    private Vector3 grapplePoint;
    private SpringJoint springJoint;
    private Vector3 currentGrapplePosition;
    private bool canShoot;

    public RigidBodyMovement rbMove;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public LayerMask whatIsNotGrappleable;
    public ParticleSystem particleShot;
    public GameObject hookPrefab;
    public Image crosshair;
    public float maxGrappleDistance = 100f;
    public int shotsLeft = 3;

    private GameObject tempProjectile;
    private bool tempProjectileAlive = false;
    public bool isPaused;

    AudioManager am;
    public Animator grappleAnimator;

    private void Awake()
    {
        am = FindObjectOfType<AudioManager>();
    }

    private void Update() {
        if (!isPaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (shotsLeft < 1)
                {
                    //particleShot.Play();
                    // Play no shot SFX
                    canShoot = false;
                }
                if (canShoot)
                {
                    
                    am.Play("grapple_rope");
                    am.Play("grapple_whizz");
                    StartGrapple();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StopGrapple();
            }
            else if (Input.GetMouseButtonDown(1) && isGrappling())
            {
                RetractGrapple();
            }
            else if (Input.GetMouseButtonUp(1) && isGrappling())
            {
                StopRetracting();
            }

                

            if (rbMove.isGrounded())
            {
                canShoot = true;
                shotsLeft = 1;
            }
            if (shotsLeft < 1)
            {
                hookPrefab.SetActive(false);
            }
            else if (canShoot && !isGrappling())
            {
                hookPrefab.SetActive(true);
            }

            // Change crosshair colour when hovering over grappleable object
            RaycastHit[] crossHits;
            crossHits = Physics.RaycastAll(cam.position, cam.forward, maxGrappleDistance);

            if(crossHits.Length == 0 || shotsLeft < 1)
            {
                crosshair.color = new Color(0, 0, 0, 0.7255f);
            }
            else
            {
                RaycastHit closestHit = crossHits[0];
                for(int i = 1; i < crossHits.Length; i++)
                {
                    RaycastHit crossHit = crossHits[i];
                    if (crossHit.distance < closestHit.distance)
                    {
                        closestHit = crossHit;
                    }
                }


                if(whatIsGrappleable == (whatIsGrappleable | (1 << closestHit.collider.gameObject.layer)))
                {
                    if (shotsLeft > 0)
                        crosshair.color = new Color(0, 1, 1, 1);
                        
                }
                else if(whatIsNotGrappleable == (whatIsNotGrappleable| (1 << closestHit.collider.gameObject.layer)))
                {
                    crosshair.color = new Color(0, 0, 0, 0.7255f);
                }
                else
                {
                    crosshair.color = new Color(0, 0, 0, 0.7255f);
                }
            }
        }
    }

    private void StartGrapple()
    {
        // Show Particle Effect
        particleShot.Play();

        // Get a raycast array of all objects hit
        RaycastHit[] crossHits;
        crossHits = Physics.RaycastAll(cam.position, cam.forward, maxGrappleDistance);

        // If we're not hovering an object, nothing to grapple to
        if(crossHits.Length == 0)
        {
            // Play shot miss SFX
            am.Play("grapple_shot");

            // Gets the players current velocity then shoots a Hook out
            Vector3 moveVelocity = rbMove.GetComponent<Rigidbody>().velocity;
            tempProjectile = Instantiate(hookPrefab, gunTip.position, gunTip.rotation);
            tempProjectile.transform.Rotate(0f, 180f, 0f);
            tempProjectile.AddComponent<Rigidbody>();
            tempProjectile.GetComponent<Rigidbody>().velocity = moveVelocity;
            tempProjectile.GetComponent<Rigidbody>().AddForce(gunTip.forward * 2500f);
            grapplePoint = tempProjectile.transform.position;
            Destroy(tempProjectile, 1.5f);
        }
        else
        {
            // Gets the closest object hit by the raycast
            RaycastHit closestHit = crossHits[0];
            for(int i = 1; i < crossHits.Length; i++)
            {
                RaycastHit crossHit = crossHits[i];
                if (crossHit.distance < closestHit.distance)
                {
                    closestHit = crossHit;
                }
            }

            // If the closest object is grappleable, grapple to it
            if(whatIsGrappleable == (whatIsGrappleable | (1 << closestHit.collider.gameObject.layer)))
            {
                am.Play("wood_impact");

                // Initialize Spring Component
                grapplePoint = closestHit.point;
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
            }
            else if(whatIsNotGrappleable == (whatIsNotGrappleable| (1 << closestHit.collider.gameObject.layer)))
            {
                // Play shot miss SFX if the layer is not grappleable
                am.Play("grapple_shot");
            }
            else
            {
                // Just in case the code above bugs out, default to a miss (sorry players)
                am.Play("grapple_shot");
            }
        }

        // No grappling more than once per airtime
        //canShoot = false;

        // Reduce shots by 1
        shotsLeft--;
        hookPrefab.SetActive(false);
    }

    private void StopGrapple()
    {
        grappleAnimator.SetTrigger("Idle");
        Destroy(springJoint);
        Destroy(tempProjectile);
    }

    private void RetractGrapple()
    {
        grappleAnimator.SetTrigger("Retract");
        springJoint.maxDistance = 2.5f;
        springJoint.minDistance = 0.25f;      
        //springJoint.spring = 10f;
    }

    private void StopRetracting()
    {
        grappleAnimator.SetTrigger("Idle");
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
        springJoint.maxDistance = distanceFromPoint * 0.9f;
        springJoint.minDistance = distanceFromPoint * 0.65f;
        springJoint.spring = 4.5f;
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
