using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerUIEvent : MonoBehaviour
{
    private UIManager _UIManager;
    [SerializeField] private UnityEvent triggerEvent;
    
    private void Start()
    {
        _UIManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerEvent.Invoke();
        }
    }

    public void DiscoverLocation(int area)
    {
        _UIManager.DiscoverArea(area);
    }
}