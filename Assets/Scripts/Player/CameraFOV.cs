using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    private Camera cam;
    private CameraShake camShake;
    private GunShake gunShake;
    private RippleEffect rippleEffect;
    [SerializeField] private float baseFOV = 65f;
    [SerializeField] private float delta = 3f;
    [SerializeField] private float zoomSpeed = 50f;
    [SerializeField] private float shakeMagnitude = 1.2f;
    [SerializeField] AnimationCurve dashFOV;

    private float currentFOV;
    private float desiredFOV;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        camShake = GetComponent<CameraShake>();
        gunShake = GetComponent<GunShake>();
        rippleEffect = GetComponent<RippleEffect>();
    }
    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = baseFOV;
        currentFOV = baseFOV;
        desiredFOV = baseFOV;
    }

    private void Update()
    {
        // Calculates what the FOV of the screen should be at the current frame 
        currentFOV = Mathf.MoveTowards(currentFOV, desiredFOV, zoomSpeed * Time.deltaTime);
        // Then sets this FOV to the main camera
        cam.fieldOfView = currentFOV;
    }

    // Used by the settings menu to change the base FOV of the screen
    public void SetBaseFOV(int newFOV)
    {
        baseFOV = newFOV;
    }

    public void GoingSlow()
    {
        //cam.fieldOfView = baseFOV;
        desiredFOV = baseFOV;
        camShake.StopShake();
    }

    public void GoingFast()
    {
        //cam.fieldOfView = baseFOV + delta;
        desiredFOV = baseFOV + delta;
        camShake.StopShake();
    }

    public void GoingFaster()
    {
        //cam.fieldOfView = baseFOV + delta * 2;
        desiredFOV = baseFOV + delta * 2;
        camShake.StopShake();
    }

    public void GoingTooFast()
    {
        desiredFOV = baseFOV + delta * 10;
        camShake.StartShake(shakeMagnitude);
    }

    public void GoDashing()
    {
        StartCoroutine(GoDashRoutine());
        rippleEffect.Emit(new Vector3(0.5f, 0.5f, 0)); // Percentage of the screen, so emits at 50%/50% - middle of screen.
        Debug.Log(transform.position);
    }

    IEnumerator GoDashRoutine()
    {
        float timer = 0f;
        Debug.Log(dashFOV[dashFOV.length - 1].time);
        while (timer <= dashFOV[dashFOV.length - 1].time)
        {
            currentFOV = desiredFOV * dashFOV.Evaluate(timer);
            yield return new WaitForSeconds(0.01f);
            timer += 0.01f;
        }
        yield return null;
    }
}
