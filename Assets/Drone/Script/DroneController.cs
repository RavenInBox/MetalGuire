using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private Transform PointA, PointB;
    [SerializeField] private float speed;
    [SerializeField] private Animator anim;

    private Transform dir;
    private bool Det = false;

    private void Start()
    {
        transform.position = PointA.position;
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (!Det)
        {
            if (transform.position == PointA.position) dir = PointB;
            if (transform.position == PointB.position) dir = PointA;
            transform.position = Vector2.MoveTowards(transform.position, dir.position, Random.Range(10, speed) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == ("Player"))
        {
            anim.Play("Detect");
            Invoke(nameof(Reset), 5f);
            Det = true;
        }
    }

    private void Reset()
    {
        anim.Play("Idle 1");
        Det = false;
    }
}
