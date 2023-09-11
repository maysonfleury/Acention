using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float baseFOV = 65f;
    [SerializeField] private float delta = 3f;
    [SerializeField] private float zoomSpeed = 50f;

    private float currentFOV;
    private float desiredFOV;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = baseFOV;
        currentFOV = baseFOV;
        desiredFOV = baseFOV;
    }

    private void Update()
    {
        ProcessFOV();
        SetFOV();
    }

    public void GoingFaster()
    {
        //cam.fieldOfView = baseFOV + delta * 2;
        desiredFOV = baseFOV + delta * 2;
    }

    public void GoingFast()
    {
        //cam.fieldOfView = baseFOV + delta;
        desiredFOV = baseFOV + delta;
    }

    public void GoingSlow()
    {
        //cam.fieldOfView = baseFOV;
        desiredFOV = baseFOV;
    }

    public void SetBaseFOV(int newFOV)
    {
        baseFOV = newFOV;
    }
 
    void ProcessFOV()
    {
        currentFOV = Mathf.MoveTowards(currentFOV, desiredFOV, zoomSpeed * Time.deltaTime);
    }
 
    void SetFOV()
    {
        cam.fieldOfView = currentFOV;
    }
}
