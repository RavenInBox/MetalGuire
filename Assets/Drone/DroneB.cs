using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneB : MonoBehaviour
{
    [SerializeField] private Transform PointA, PointB, Target;
    [SerializeField] private float speed, timeW, timeS, temp;
    [SerializeField] private Animator anim;
    private bool Det = false;

    private void Start()
    {
        transform.position = PointA.position;
        temp = timeW;
        timeW = 5f;
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (timeW < 0)
        {
            if (transform.position.x == Target.position.x)
            {
                Det = true;
                anim.Play("Detect");
            }

            if (!Det)
            {
                var tar = new Vector2(Target.position.x, transform.position.y );
                transform.position = Vector2.MoveTowards(transform.position, tar, speed * Time.deltaTime);
            }
        }
        else
        {
            timeW -= Time.deltaTime * timeS;
            transform.position = Vector2.MoveTowards(transform.position, PointA.position, speed * Time.deltaTime);
        }
    }

    public void Reset()
    {
        Det = false;
        if (temp > 5) temp -= 2f;
        timeW = temp;
    }
}
