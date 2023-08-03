using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hedron : MonoBehaviour
{
    [SerializeField] private AudioClip _destroySound;
    [SerializeField] public bool isDestroyable = false;

    [SerializeField] public bool doesRotate = false;
    
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _speed;
    
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
            AudioSource.PlayClipAtPoint(_destroySound, transform.position);
            Debug.Log("Hedron Destroyed");
            gameObject.SetActive(false);
        }
    }

    public void SpawnHedron()
    {

    }
}
