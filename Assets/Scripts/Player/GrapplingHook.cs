using System;
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
    public bool EnableAimAssist;
    public LayerMask whatIsGrappleable;
    public LayerMask whatIsNotGrappleable;
    public ParticleSystem particleShot;
    public GameObject hookPrefab;
    public Image crosshair;
    public GameObject crossLeft;
    public GameObject crossRight;
    public GameObject crossUp;
    public GameObject crossDown;
    public List<Image> outerCrosshairs;
    public AnimationCurve crosshairDistanceCurve;
    public Color grappleableColor;
    public Color nongrappleableColor;
    public float maxGrappleDistance = 100f;
    public int shotsLeft = 99;

    private GameObject tempProjectile;
    public bool isPaused;

    AudioManager am;
    public Animator grappleAnimator;

    private RaycastHit sphereHit;
    private RaycastHit raycastHit;
    private RaycastHit nonHit;
    private Transform predictedHitPoint;
    public float sphereCastRadius = 1f;
    public float aimAssistCastRadius = 5f;
    public GameObject sphere;

    private void Awake()
    {
        am = FindObjectOfType<AudioManager>();
        sphere.SetActive(false);
    }

    private void Update() {
        if (!isPaused)
        {
            GetInput();
            GetAiming();
            DrawDynamicCrosshair();
            
            //if (rbMove.grounded || rbMove.isBouncing())
            //{
            //    _canShoot = true;
            //    shotsLeft = 1000;
            //}

            // If grappling a moving object, update the grapple position and spring joint
            if(_isHedroned)
            {
                grapplePoint = objectWorldPos.position;
                springJoint.connectedAnchor = grapplePoint;
            }
        }
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //if (shotsLeft < 1)
            //{
            //    //particleShot.Play();
            //    //TODO: Play no shot SFX
            //    _canShoot = false;
            //}
            //if (_canShoot)
            //{
                am.Play("grapple_rope");
                am.Play("grapple_whizz");
                StartGrapple();
            //}
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
    }

    private void GetAiming()
    {
        // SphereCast for Aim Assist, RayCast for regular aiming
        if (EnableAimAssist)
        {
            Physics.Raycast(cam.position, cam.forward, out raycastHit, maxGrappleDistance, whatIsGrappleable);
            Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out sphereHit, maxGrappleDistance, whatIsGrappleable);
            Physics.SphereCast(cam.position, sphereCastRadius, cam.forward, out nonHit, maxGrappleDistance, whatIsNotGrappleable);
        }
        else
        {
            Physics.Raycast(cam.position, cam.forward, out raycastHit, maxGrappleDistance, whatIsGrappleable);
            Physics.Raycast(cam.position, cam.forward, out sphereHit, maxGrappleDistance * 2f, whatIsGrappleable);
            Physics.Raycast(cam.position, cam.forward, out nonHit, maxGrappleDistance * 2f, whatIsNotGrappleable);
        }

        // Check if a nongrappleable is covering a grappleable
        if (nonHit.point != Vector3.zero && raycastHit.point != Vector3.zero)
            if (nonHit.distance < raycastHit.distance)
                raycastHit.point = Vector3.zero;
        if (nonHit.point != Vector3.zero && sphereHit.point != Vector3.zero)
            if (nonHit.distance < sphereHit.distance)
                sphereHit.point = Vector3.zero;
    }

    private void StartGrapple()
    {
        // Show Particle Effect and remove Hook
        particleShot.Play();
        hookPrefab.SetActive(false);

        // If we're not hovering an object, nothing to grapple to
        if(raycastHit.point == Vector3.zero)
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

            if (EnableAimAssist && sphereHit.point != Vector3.zero)
            {
                // Initialize Spring Component
                grapplePoint = sphereHit.point;
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
        }
        else
        {
            switch (raycastHit.collider.gameObject.tag)
            {
                case "Wood":
                    am.Play("wood_impact");
                break;

                case "Crystal":
                    am.Play("crystal_sound");
                break;

                case "Mushroom":
                    am.Play("shroom_impact");
                break;

                case "Leaf":
                    Debug.Log("Lief Debugson");
                break;
            }

            // If we hit a Hedron
            if(raycastHit.collider.gameObject.GetComponent<Hedron>() != null)
            {
                var hedron = raycastHit.collider.gameObject.GetComponent<Hedron>();
                
                if(hedron.isDestroyable)
                {
                    _isDestructing = true;
                    _grappledObject = raycastHit.collider.gameObject;
                }
                if(hedron.doesRotate)
                {
                    _isHedroned = true;
                    objectWorldPos = raycastHit.transform;
                }
            }

            // Initialize Spring Component
            grapplePoint = raycastHit.point;
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

    private void DrawDynamicCrosshair()
    {
        // Set crosshair to default state
        crosshair.color = new Color(0, 0, 0, 0.45f);
        crossLeft.SetActive(false);
        crossRight.SetActive(false);
        crossUp.SetActive(false);
        crossDown.SetActive(false);
        // Hit point visual
        sphere.SetActive(false);

        // If we're already grappling, no need to have dynamic crosshair
        if (isGrappling())
            crosshair.color = new Color(0, 0.3f, 0.3f, 0.7f); // Dim Cyan
        else if(raycastHit.point != Vector3.zero && shotsLeft > 0)
        {
            // Bright Cyan crosshair if we can grapple and we're not already
            crosshair.color = new Color(0, 1, 1, 1f);
        }
        else if (sphereHit.point != Vector3.zero && shotsLeft > 0)
        {
            Vector3 direction = Vector3.zero;
            if (EnableAimAssist)
            {
                sphere.SetActive(true);
                sphere.transform.position = sphereHit.point;
                Vector3 centerOfScreen = new Vector3(Screen.width/2, Screen.height/2);
                Vector3 screenHitPoint = Camera.main.WorldToScreenPoint(sphereHit.point);
                direction = (screenHitPoint - centerOfScreen) / 2;
            }

            crossLeft.SetActive(true);
            crossRight.SetActive(true);
            crossUp.SetActive(true);
            crossDown.SetActive(true);

            float delta = crosshairDistanceCurve.Evaluate(sphereHit.distance / 100f);
            crossLeft.transform.localPosition = new Vector3(-delta + direction.x, direction.y);
            crossRight.transform.localPosition = new Vector3(delta + direction.x, direction.y);
            crossUp.transform.localPosition = new Vector3(direction.x, direction.y + delta);
            crossDown.transform.localPosition = new Vector3(direction.x, direction.y - delta);

            foreach (Image cros in outerCrosshairs)
                cros.color = grappleableColor;
            crosshair.color = new Color(0, 0, 0, 0.7f); // Dim black when hovering an out-of-range grappleable
        }
        else if (nonHit.point != Vector3.zero || shotsLeft <= 0)
        {
            Vector3 direction = Vector3.zero;
            if (EnableAimAssist)
            {
                sphere.SetActive(true);
                sphere.transform.position = nonHit.point;
                Vector3 centerOfScreen = new Vector3(Screen.width/2, Screen.height/2);
                Vector3 screenHitPoint = Camera.main.WorldToScreenPoint(nonHit.point);
                direction = (screenHitPoint - centerOfScreen) / 2;
            }

            crossLeft.SetActive(true);
            crossRight.SetActive(true);
            crossUp.SetActive(true);
            crossDown.SetActive(true);

            float delta = crosshairDistanceCurve.Evaluate(nonHit.distance / 100f);
            crossLeft.transform.localPosition = new Vector3(-delta + direction.x, direction.y);
            crossRight.transform.localPosition = new Vector3(delta + direction.x, direction.y);
            crossUp.transform.localPosition = new Vector3(direction.x, direction.y + delta);
            crossDown.transform.localPosition = new Vector3(direction.x, direction.y - delta);
            
            foreach (Image cros in outerCrosshairs)
                cros.color = nongrappleableColor;
            crosshair.color = new Color(0.4f, 0, 0, 0.7f); // Dim red when we can't grapple
        }
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