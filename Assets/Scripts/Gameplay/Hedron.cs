using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

public class Hedron : MonoBehaviour
{
    [SerializeField] public bool isDestroyable = false;
    [ShowIf("isDestroyable")] [SerializeField] private AudioClip _destroySound;
    [ShowIf("isDestroyable")] [SerializeField] private UnityEvent _onDestroyEvent;

    [SerializeField] public bool doesRotate = false;
    
    [ShowIf("doesRotate")] [SerializeField] private Vector3 _rotation;
    [ShowIf("doesRotate")] [SerializeField] private float _speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(doesRotate) transform.Rotate(_rotation * _speed * Time.deltaTime);
    }

    public void DestroyHedron()
    {
        if(isDestroyable)
        {
            _onDestroyEvent.Invoke();
            AudioSource.PlayClipAtPoint(_destroySound, transform.position);
            Debug.Log("Hedron Destroyed");
            gameObject.SetActive(false);
        }
    }

    public void SpawnHedron()
    {

    }
}
