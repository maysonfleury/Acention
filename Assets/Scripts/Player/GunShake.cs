using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GunShake : MonoBehaviour
{
    [SerializeField] private bool _enableGunShake = true;
    [SerializeField, Range(0, 0.1f)] private float _Amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private int _frequency = 10;
    [SerializeField, Range(0, 90)] private float _randomness = 10.0f;

    [SerializeField] private Transform _Gun = null;

    private void StartGunShake(float duration)
    {
        _Gun.DOShakePosition(duration, _Amplitude, _frequency, _randomness, false, true, ShakeRandomnessMode.Full);
    }

    private void StopGunShake()
    {
        if (DOTween.IsTweening(_Gun))
            DOTween.Kill(_Gun);
    }
}
