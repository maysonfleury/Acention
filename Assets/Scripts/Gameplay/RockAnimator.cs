using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockAnimator : MonoBehaviour
{
    public enum MovementTypes { Idle, Float, Bounce, Translate }

    public MovementTypes movement;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger(movement.ToString());
    }
}
