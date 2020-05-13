using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int jumpPower;
    public float moveSpeedZ; // left, right move speed
    GameObject[] roads;  // use to computes moveLeft, moveRight
    public GameObject road0;
    public GameObject road1;
    public GameObject road2;
    public GameObject road3;
    public GameObject road4;
    

    int currentPos; // current pos of player's car ([0, 4])
    int targetPos;
    int currentLife; // current life of player

    bool isGround;
    bool doJump;
    bool doMoveLeft;
    bool doMoveRight;

    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        roads = new GameObject[5];
        roads[0] = road0;
        roads[1] = road1;
        roads[2] = road2;
        roads[3] = road3;
        roads[4] = road4;

        currentPos = 2;  // initial pos
        targetPos = 2;
        currentLife = 3;

        doJump = false;
        doMoveLeft = false;
        doMoveRight = false;
    }

    void Update()
    {
        // TODO: collision check (obstacle or item)

        if (doMoveLeft || doMoveRight)
        {
            Vector3 newPos = transform.position;
            float delta = Math.Min(Math.Abs(roads[targetPos].transform.position.z - newPos.z), moveSpeedZ);
            newPos.z = (doMoveLeft) ? (newPos.z - delta) : (newPos.z + delta);
            transform.position = newPos;
            if(roads[targetPos].transform.position.z == transform.position.z)
            {
                doMoveLeft = doMoveRight = false;
                currentPos = targetPos;
            }
        }   
        // input check, move car (left, right, jump)
        if(!doMoveLeft && !doMoveRight && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow) && currentPos != 0)
            {
                doMoveLeft = true;
                targetPos = currentPos - 1;
            } else if(Input.GetKeyDown(KeyCode.RightArrow) && currentPos != 4)
            {
                doMoveRight = true;
                targetPos = currentPos + 1;
            } 
        }

        if(isGround && Input.GetKeyDown(KeyCode.Space))
        {
            doJump = true;
        }
    }

    void FixedUpdate()
    {
        if(doJump && isGround) 
        {
            isGround = false;
            jump();
        }

        doJump = false;
    }

    private void jump()
    {
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road"))
        {
            isGround = true;
        }
    }
}
