using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBob : MonoBehaviour
{
    [SerializeField] private bool _enableGunBob = true;

    [SerializeField, Range(0, 0.1f)] private float _Amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float _frequency = 10.0f;

    [SerializeField] private Transform _Gun = null;

    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;
    private RigidBodyMovement _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponentInParent<RigidBodyMovement>();
        _startPos = _Gun.localPosition;
    }

    void Update()
    {
        if (!_enableGunBob) return;
        if (!_rigidbody.grounded) return;
        CheckMotion();
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
        _Gun.localPosition += motion;
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _Amplitude / 20f;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _Amplitude / 10f;
        return pos;
    }

    private void ResetPosition()
    {
        if (_Gun.localPosition == _startPos) return;
        _Gun.localPosition = Vector3.Lerp(_Gun.localPosition, _startPos, 10 * Time.deltaTime);
    }
}
