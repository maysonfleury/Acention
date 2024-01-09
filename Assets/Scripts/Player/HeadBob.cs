using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private bool _enableHeadBob = true;

    [SerializeField, Range(0, 0.1f)] private float _Amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float _frequency = 10.0f;

    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;

    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;
    private RigidBodyMovement _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<RigidBodyMovement>();
        _startPos = _camera.localPosition;
    }

    void Update()
    {
        if (!_enableHeadBob) return;
        CheckMotion();
        _camera.LookAt(FocusTarget());
    }

    private void CheckMotion()
    {
        ResetPosition();
        var veloMag = _rigidbody.FindVelRelativeToLook();
        float velocity = Mathf.Abs(veloMag.x) + Mathf.Abs(veloMag.y);
        if (velocity < _toggleSpeed) return;
        if (!_rigidbody.grounded) return;

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _Amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _Amplitude * 2;
        return pos;
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
        pos += _cameraHolder.forward * 15.0f;
        return pos;
    }

    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }
}