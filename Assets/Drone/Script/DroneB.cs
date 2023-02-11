using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DroneB : MonoBehaviour
{
    [SerializeField] private Transform ReturnPoint, Target;
    [SerializeField] private float speed;
    [SerializeField] private Animator anim;

    public bool targetPositionCheck = false;
    private Vector2 direction;
    public int state;

    private void Start()
    {
        transform.position = ReturnPoint.position;
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        if (transform.position.x.Equals(direction.x))
            targetPositionCheck = true;

        if (!targetPositionCheck) transform.position =
                Vector2.MoveTowards(transform.position, direction, speed * Time.deltaTime);

        if (state == 0) direction = new Vector2(Target.position.x, transform.position.y);
        else direction = new Vector2(ReturnPoint.position.x, transform.position.y);

        if (direction.x.Equals(Target.position.x)) anim.Play("Detect");
        else
        {
            anim.Play("Idle");
            Reset();
        }
    }

    private void Reset()
    {
        if (state > 0) state--; else state++;
        targetPositionCheck = false;
    }
}
