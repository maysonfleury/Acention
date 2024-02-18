using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GrappleRope))]
public class GrapplingHook : MonoBehaviour
{
    private Vector3 grapplePoint;
    private SpringJoint springJoint;
    private bool _canShoot;
    private bool _isHedroned;
    private bool _isDestructing;
    private GameObject _grappledObject;
    private Transform objectWorldPos;

    public RigidBodyMovement rbMove;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public ParticleSystem particleShot;
    public GameObject hookPrefab;
    public Image crosshair;
    public GameObject crossLeft;
    public GameObject crossRight;
    public GameObject crossUp;
    public GameObject crossDown;
    public AnimationCurve crosshairDistanceCurve;
    public float maxGrappleDistance = 100f;
    public int shotsLeft = 99;

    private GameObject tempProjectile;
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
                    //TODO: Play no shot SFX
                    _canShoot = false;
                }
                if (_canShoot)
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
            
            // TODO: TAKE THIS TF OUT OF UPDATE!
            // Resets shots to 1000 whenever you touch the ground
            if (rbMove.grounded || rbMove.isBouncing())
            {
                _canShoot = true;
                //shotsLeft = 1000;
            }

            // If grappling a moving object, update the grapple position and spring joint
            if(_isHedroned)
            {
                grapplePoint = objectWorldPos.position;
                springJoint.connectedAnchor = grapplePoint;
            }

            if (isGrappling())
            {
                // Set crosshair to grappling state
                crosshair.color = new Color(0, 1, 1, 0.45f);
                crossLeft.SetActive(false);
                crossRight.SetActive(false);
                crossUp.SetActive(false);
                crossDown.SetActive(false);
            }
            else
            {
                // RayCast forward to see the surface you're looking at
                RaycastHit[] crossHits;
                crossHits = Physics.RaycastAll(cam.position, cam.forward, maxGrappleDistance + 50f, whatIsGrappleable);

                // If we hit nothing within our grapple range, set crosshair to default state
                if(crossHits.Length == 0 || shotsLeft < 1)
                {
                    crosshair.color = new Color(0, 0, 0, 0.45f);
                    crossLeft.SetActive(false);
                    crossRight.SetActive(false);
                    crossUp.SetActive(false);
                    crossDown.SetActive(false);
                    crossLeft.transform.localPosition = new Vector3(0, 0, 0);
                    crossRight.transform.localPosition = new Vector3(0, 0, 0);
                    crossUp.transform.localPosition = new Vector3(0, 0, 0);
                    crossDown.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    // Put every grappleable hit within range into array.
                    // Since raycast hit arrays are unordered, 
                    // we loop through once to grab the closest surface
                    RaycastHit closestHit = crossHits[0];
                    for(int i = 1; i < crossHits.Length; i++)
                    {
                        RaycastHit crossHit = crossHits[i];
                        if (crossHit.distance < closestHit.distance)
                        {
                            if (whatIsGrappleable == (whatIsGrappleable | (1 << closestHit.collider.gameObject.layer)))
                            {
                                closestHit = crossHit;
                                break;
                            }
                        }
                    }

                    if (closestHit.distance <= maxGrappleDistance)
                    {
                        crosshair.color = new Color(0, 1, 1, 1);
                        crossLeft.SetActive(false);
                        crossRight.SetActive(false);
                        crossUp.SetActive(false);
                        crossDown.SetActive(false);
                    }
                    else if (closestHit.distance <= maxGrappleDistance + 50f)
                    {
                        // Dynamic crosshair when out of range <3
                        crosshair.color = new Color(0, 0, 0, 0.7255f);
                        crossLeft.SetActive(true);
                        crossRight.SetActive(true);
                        crossUp.SetActive(true);
                        crossDown.SetActive(true);
                        float delta = crosshairDistanceCurve.Evaluate(closestHit.distance / 100f);
                        crossLeft.transform.localPosition = new Vector3(-delta, 0);
                        crossRight.transform.localPosition = new Vector3(delta, 0, 0);
                        crossUp.transform.localPosition = new Vector3(0, delta, 0);
                        crossDown.transform.localPosition = new Vector3(0, -delta, 0);
                    }
                }
            }
        }
    }

    private void StartGrapple()
    {
        // Show Particle Effect and remove Hook
        particleShot.Play();
        hookPrefab.SetActive(false);

        // Get a raycast array of all objects hit
        RaycastHit[] crossHits;
        crossHits = Physics.RaycastAll(cam.position, cam.forward, maxGrappleDistance,  whatIsGrappleable);

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
            tempProjectile.GetComponent<Rigidbody>().AddForce(gunTip.forward * 4500f);
            grapplePoint = tempProjectile.transform.position;
            Destroy(tempProjectile, 1.5f);
        }
        else
        {
            // Put every grappleable hit within range into array.
            // Since raycast hit arrays are unordered, 
            // we loop through once to grab the closest hit
            RaycastHit closestHit = crossHits[0];
            for(int i = 1; i < crossHits.Length; i++)
            {
                RaycastHit crossHit = crossHits[i];
                if (crossHit.distance < closestHit.distance)
                {
                    if(whatIsGrappleable == (whatIsGrappleable | (1 << closestHit.collider.gameObject.layer)))
                    {
                        closestHit = crossHit;
                        break;
                    }
                }
            }

            if(closestHit.collider.gameObject.CompareTag("Wood"))
            {
                Debug.Log("Wooud");
                am.Play("wood_impact");
            }
            else if(closestHit.collider.gameObject.CompareTag("Crystal"))
            {
                am.Play("crystal_sound");
                Debug.Log("Crystal");
            }
            else if(closestHit.collider.gameObject.CompareTag("Mushroom"))
            {
                am.Play("shroom_impact");
                Debug.Log("MooshMoosh");
            }
            else if(closestHit.collider.gameObject.CompareTag("Leaf"))
            {
                Debug.Log("Lief Debugson");
            }
            // If we hit a Hedron
            if(closestHit.collider.gameObject.GetComponent<Hedron>() != null)
            {
                var hedron = closestHit.collider.gameObject.GetComponent<Hedron>();
                
                if(hedron.isDestroyable)
                {
                    _isDestructing = true;
                    _grappledObject = closestHit.collider.gameObject;
                }
                if(hedron.doesRotate)
                {
                    _isHedroned = true;
                    objectWorldPos = closestHit.transform;
                }
            }

            // Initialize Spring Component
            grapplePoint = closestHit.point;
            springJoint = player.gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;
            // How far the rope will stretch
            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            springJoint.maxDistance = distanceFromPoint * 0.9f;
            springJoint.minDistance = 0f;
            // How the rope feels
            springJoint.spring = 4.5f;
            springJoint.damper = 5f;
            springJoint.massScale = 4.5f;
        }

        // No grappling more than once per airtime
        //_canShoot = false;

        // Reduce shots by 1
        //shotsLeft--;
        hookPrefab.SetActive(false);
    }

    private void StopGrapple()
    {
        if(_isDestructing)
        {
            _grappledObject.GetComponent<Hedron>().DestroyHedron();
        }
        _isHedroned = false;
        _isDestructing = false;
        hookPrefab.SetActive(true);
        grappleAnimator.SetTrigger("Idle");
        Destroy(springJoint);
        Destroy(tempProjectile);
    }

    private void RetractGrapple()
    {
        grappleAnimator.SetTrigger("Retract");
        springJoint.maxDistance = 0.05f;     
        springJoint.spring = 5f;
    }

    private void StopRetracting()
    {
        grappleAnimator.SetTrigger("Idle");
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
        springJoint.maxDistance = distanceFromPoint * 0.9f;
        springJoint.spring = 4.5f;
    }

    public bool isGrappling()
    {
        return springJoint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
