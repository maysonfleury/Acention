using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerEvent.Invoke();
        }
    }
}
