using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform playerHead;

    void LateUpdate() {
        transform.position = playerHead.transform.position;
    }
}