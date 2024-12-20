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
    public AnimationCurve crosshairOpacityCurve;
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
        else
        {
            if (springJoint != null) StopGrapple();
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
            Physics.Raycast(cam.position, cam.forward, out sphereHit, maxGrappleDistance * 2f, whatIsGrappleable); // sphereHit is not actually a spherecast if we're not using aim assist
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
                    rbMove.ResetDashImmediate();
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
        // If we're already grappling
        if (isGrappling())
        {
            // Dim Cyan inner crosshair
            crosshair.color = new Color(0, 0.3f, 0.3f, 0.7f);

            // Set Outer crosshair position and color
            float delta = crosshairDistanceCurve.Evaluate(0f);
            crossLeft.transform.localPosition = new Vector3(-delta, 0);
            crossRight.transform.localPosition = new Vector3(delta, 0);
            crossUp.transform.localPosition = new Vector3(0, delta);
            crossDown.transform.localPosition = new Vector3(0, -delta);
            foreach (Image cros in outerCrosshairs)
                cros.color = new Color(0, 0.4f, 0.4f, 0.7f); // Dim cyan outer for visibility
        }
        else if(raycastHit.point != Vector3.zero && shotsLeft > 0)
        {
            // Bright Cyan crosshair if we can grapple and we're not already
            crosshair.color = new Color(0, 1, 1, 1f);

            // Set Outer crosshair position and color
            float delta = crosshairDistanceCurve.Evaluate(0f);
            crossLeft.transform.localPosition = new Vector3(-delta, 0);
            crossRight.transform.localPosition = new Vector3(delta, 0);
            crossUp.transform.localPosition = new Vector3(0, delta);
            crossDown.transform.localPosition = new Vector3(0, -delta);
            foreach (Image cros in outerCrosshairs)
            {
                // Dim cyan outer for visibility with opacity depending on distance
                cros.color = new Color(0, 0.4f, 0.4f, 0.7f);
            }
            
        }
        else if (sphereHit.point != Vector3.zero && shotsLeft > 0)
        {
            // Dim black when hovering an out-of-range grappleable
            crosshair.color = new Color(0, 0, 0, 0.7f);

            Vector3 direction = Vector3.zero;
            if (EnableAimAssist)
            {
                sphere.SetActive(true);
                sphere.transform.position = sphereHit.point;
                Vector3 centerOfScreen = new Vector3(Screen.width/2, Screen.height/2);
                Vector3 screenHitPoint = Camera.main.WorldToScreenPoint(sphereHit.point);
                direction = (screenHitPoint - centerOfScreen) / 2;
            }
            float delta = crosshairDistanceCurve.Evaluate(sphereHit.distance / 100f);
            float alpha = crosshairOpacityCurve.Evaluate(delta);
            crossLeft.transform.localPosition = new Vector3(-delta + direction.x, direction.y);
            crossRight.transform.localPosition = new Vector3(delta + direction.x, direction.y);
            crossUp.transform.localPosition = new Vector3(direction.x, direction.y + delta);
            crossDown.transform.localPosition = new Vector3(direction.x, direction.y - delta);
            foreach (Image cros in outerCrosshairs)
            {
                Color outerColor = grappleableColor;
                outerColor.a = alpha;
                cros.color = outerColor;
            }
        }
        else if (nonHit.point != Vector3.zero || shotsLeft <= 0)
        {
            // Dim red when we can't grapple
            crosshair.color = new Color(0.4f, 0, 0, 0.7f);

            Vector3 direction = Vector3.zero;
            if (EnableAimAssist)
            {
                sphere.SetActive(true);
                sphere.transform.position = nonHit.point;
                Vector3 centerOfScreen = new Vector3(Screen.width/2, Screen.height/2);
                Vector3 screenHitPoint = Camera.main.WorldToScreenPoint(nonHit.point);
                direction = (screenHitPoint - centerOfScreen) / 2;
            };
            float delta = crosshairDistanceCurve.Evaluate(nonHit.distance / 100f);
            float alpha = crosshairOpacityCurve.Evaluate(delta);
            crossLeft.transform.localPosition = new Vector3(-delta + direction.x, direction.y);
            crossRight.transform.localPosition = new Vector3(delta + direction.x, direction.y);
            crossUp.transform.localPosition = new Vector3(direction.x, direction.y + delta);
            crossDown.transform.localPosition = new Vector3(direction.x, direction.y - delta);
            foreach (Image cros in outerCrosshairs)
            {
                Color outerColor = nongrappleableColor;
                outerColor.a = alpha;
                cros.color = outerColor;
            }
        }
        else
        {
            // Set crosshair to default state
            crosshair.color = new Color(0, 0, 0, 0.45f);
            float delta = crosshairDistanceCurve.Evaluate(0f);
            crossLeft.transform.localPosition = new Vector3(-delta, 0);
            crossRight.transform.localPosition = new Vector3(delta, 0);
            crossUp.transform.localPosition = new Vector3(0, delta);
            crossDown.transform.localPosition = new Vector3(0, delta);
            foreach (Image cros in outerCrosshairs)
                cros.color = new Color(0, 0, 0, 0);
            // Hit point visual
            sphere.SetActive(false);
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