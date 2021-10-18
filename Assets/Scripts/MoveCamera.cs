using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform playerHead;

    void Update() {
        transform.position = playerHead.transform.position;
    }
}
