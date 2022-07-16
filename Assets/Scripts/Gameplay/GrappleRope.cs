using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    private RopeSpring spring;

    public GrapplingHook grappleHook;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve effectCurve;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        spring = new RopeSpring();
        spring.SetTarget(0);
    }

    private void LateUpdate() {
        DrawLineRenderer();
    }

    private void DrawLineRenderer()
    {
        // Don't draw if we're not grappled
        if(!grappleHook.isGrappling())
        {
            currentGrapplePosition = grappleHook.gunTip.position;
            spring.Reset();
            if(lr.positionCount > 0)
            {
                lr.positionCount = 0;
            }
            return;
        }

        if(lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = grappleHook.GetGrapplePoint();
        var gunTipPos = grappleHook.gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPos).normalized) * Vector3.up;
        
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 10f);

        for(int i = 0; i < quality + 1; i++)
        {
            var delta = i / (float) quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * effectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTipPos, currentGrapplePosition, delta) + offset);
        }
    }
}
