using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    private Vector3 spawnPos;
    public float speed;
    public float height = 5;

    private Vector3 PointUp;
    private Vector3 PointDown;
    private float tiem;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;
        PointUp = new Vector3(spawnPos.x, spawnPos.y + height, spawnPos.z);
        PointDown = new Vector3(spawnPos.x, spawnPos.y - height, spawnPos.z);
        StartCoroutine(GoDown());
    }

    // Update is called once per frame
    void Update()
    {
        //float y = spawnPos.y + height*Mathf.Sin(Time.deltaTime * speed);
        //transform.position = new Vector3(spawnPos.x, y, spawnPos.z);   
        tiem += Time.deltaTime * speed;
    }

    IEnumerator GoDown()
    {
        transform.position = Vector3.Lerp(spawnPos, PointDown, tiem);
        if(transform.position == PointDown)
        {
            StartCoroutine(GoToSpawn());
            yield return null;
        }
    }

    IEnumerator GoUp()
    {
        transform.position = Vector3.Lerp(spawnPos, PointUp, tiem);
        if(transform.position == PointUp)
        {
            StartCoroutine(GoToSpawn());
            yield return null;
        }
    }

    IEnumerator GoToSpawn()
    {
        transform.position = Vector3.Lerp(transform.position, spawnPos, tiem);
        if(transform.position == spawnPos)
        {
            StartCoroutine(GoUp());
            yield return null;
        }
    }
}
