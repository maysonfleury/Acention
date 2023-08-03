using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedronSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _hedronPrefabs;
    [SerializeField] private float _spawnDelay = 1f;

    [SerializeField] private float playbackVolume = 1f;
    
    [SerializeField] private AudioClip _spawnSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(SpawnHedrons());
        }
    }

    // couroutine that spawns the hedron prefabs in sequentially after a delay
    IEnumerator SpawnHedrons()
    {
        foreach (GameObject hedron in _hedronPrefabs)
        {
            if(!hedron.activeSelf) // if the hedron has been destroyed, spawn it
            {
                Debug.Log("Spawning Hedron");
                hedron.SetActive(true);
                PlayClipAt(_spawnSound, hedron.transform.position, playbackVolume);
                yield return new WaitForSeconds(_spawnDelay);
            }
            else
            {
                Debug.Log("Hedron is still alive");
            }
        }
    }

    AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float volume)
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        // set other aSource properties here, if desired
    
        aSource.rolloffMode = AudioRolloffMode.Linear;
        aSource.maxDistance = 250f;
        aSource.spatialBlend = 1f;
        aSource.volume = volume;
    
        aSource.Play(); // start the sound
        Destroy(tempGO, clip.length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }  
}
