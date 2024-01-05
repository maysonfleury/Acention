using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRings : MonoBehaviour
{

    [SerializeField] private List<GameObject> dashRings;
    [SerializeField] private GameObject lockedRings;

    private void Start()
    {
        foreach(GameObject go in dashRings)
        {
            go.SetActive(false);
        }
    }

    public void Lock()
    {
        //Debug.Log("Locking Dash Rings");
        lockedRings.SetActive(true);
    }

    public void Unlock()
    {
        //Debug.Log("Unlocking Dash Rings");
        lockedRings.SetActive(false);
    }

    public void SetRings(int level)
    {
        if(level == 0)
        {
            foreach(GameObject go in dashRings)
            {
                go.SetActive(false);
            }
        }

        else if(level == 1)
        {
            dashRings[0].SetActive(true);
            dashRings[1].SetActive(false);
            dashRings[2].SetActive(false);
            dashRings[3].SetActive(true);
            dashRings[4].SetActive(false);
            dashRings[5].SetActive(false);
        }

        else if(level == 2)
        {
            dashRings[0].SetActive(true);
            dashRings[1].SetActive(true);
            dashRings[2].SetActive(false);
            dashRings[3].SetActive(true);
            dashRings[4].SetActive(true);
            dashRings[5].SetActive(false);
        }

        if(level == 3)
        {
            foreach(GameObject go in dashRings)
            {
                go.SetActive(true);
            }
        }
    }
}
