﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;

    public int jumpPower;
    public float moveSpeedZ; // left, right move speed
    int[] roads;  // use to computes moveLeft, moveRight

    GameObject[] wheels;
    public GameObject boostEffect;
    public GameObject frontRightWheel;
    public GameObject frontLeftWheel;
    public GameObject backRightWheel;
    public GameObject backLeftWheel;

    int maxSpeed = 25;
    int currentPos; // current pos of player's car ([0, 4])
    int targetPos;
    int currentLife; // current life of player
    int currentSpeed; // current speed of player

    bool isGround;
    bool doJump;
    bool doMoveLeft;
    bool doMoveRight;
    bool isRestoring;
    bool isFinish;

    Rigidbody rigidbody;
    MeshRenderer[] meshRenderers;
    Renderer[] renderers;
    float[] rendererAlphas;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        renderers = GetComponentsInChildren<Renderer>();
        rendererAlphas = new float[renderers.Length];
        for(int i = 0; i < renderers.Length; ++i)
        {
            rendererAlphas[i] = renderers[i].material.color.a;
        }
        roads = new int[] { 2, 4, 6, 8, 10};

        wheels = new GameObject[4];
        wheels[0] = frontLeftWheel;
        wheels[1] = frontRightWheel;
        wheels[2] = backLeftWheel;
        wheels[3] = backRightWheel;
        isFinish = true;

        ResetState();
    }

    void ResetState()
    {
        currentPos = 2;  // initial pos
        targetPos = 2;
        currentLife = 3;

        doJump = false;
        doMoveLeft = false;
        doMoveRight = false;
        isRestoring = false;

        boostEffect.SetActive(false);
    }

    void Update()
    {
        // TODO: collision check (obstacle or item)

        if (doMoveLeft || doMoveRight)
        {
            Vector3 newPos = transform.position;
            float delta = Math.Min(Math.Abs(roads[targetPos] - newPos.z), moveSpeedZ * Time.deltaTime * 150);
            newPos.z = (doMoveLeft) ? (newPos.z - delta) : (newPos.z + delta);
            transform.position = newPos;
            if(roads[targetPos] == transform.position.z)
            {
                doMoveLeft = doMoveRight = false;
                currentPos = targetPos;
            }
        }   
        // input check, move car (left, right, jump)
        if(!doMoveLeft && !doMoveRight && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && !isFinish)
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
            if (doMoveLeft || doMoveRight) StartCoroutine(RotateLR());
        }

        if(isGround && Input.GetKeyDown(KeyCode.Space) && !isFinish)
        {
            doJump = true;
        }

        // rotate wheel
        if(!isFinish)
        {
            currentSpeed = StageManager.instance.GetCurrentSpeed();
            for (int i = 0; i < 4; ++i)
            {
                wheels[i].transform.Rotate(new Vector3(1, 0, 0) * currentSpeed * 30 * Time.deltaTime);
            }
        }
       
    }

    void FixedUpdate()
    {
        if(doJump && isGround) 
        {
            StartCoroutine(RotateJump());
            isGround = false;
            Jump();
        }

        doJump = false;
    }

    private void Jump()
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

    public int GetMaxSpeed()
    {
        return maxSpeed;
    }

    public bool GetRestoring()
    {
        return this.isRestoring;
    }

    public void SetRestoring(bool isRestoring)
    {
        this.isRestoring = isRestoring;
    }

    public void SetTransparent(bool transparent)
    {
        for(int i = 0; i < meshRenderers.Length; ++i)
        {
            if (transparent) meshRenderers[i].GetComponent<Renderer>().enabled = false;
            else meshRenderers[i].GetComponent<Renderer>().enabled = true;
        }
        for(int i = 0; i < renderers.Length; ++i)
        {
            if (transparent) renderers[i].material.color = new Color(renderers[i].material.color.r, renderers[i].material.color.g, renderers[i].material.color.b, 0);
            else renderers[i].material.color = new Color(renderers[i].material.color.r, renderers[i].material.color.g, renderers[i].material.color.b, rendererAlphas[i]);
        }
    }
    
    public void ShowBoost(bool isShow)
    {
        boostEffect.SetActive(isShow);
    }

    public void SetFinish(bool isFinish)
    {
        this.isFinish = isFinish;
    }

    public void Rotate(Vector3 v3)
    {
        transform.Rotate(v3);
    }

    public void Ready()
    {
        ResetState();
        transform.position = new Vector3(7.5f, 0, 6);
        transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
    }

    IEnumerator RotateLR()
    {
        if(doMoveLeft)
        {
            for (int i = 0; i < 5; ++i) // rotate
            {
                transform.Rotate(new Vector3(0, -1, 0) * 3);
                yield return new WaitForSeconds(0.01f);
            }
            for (int i = 0; i < 5; ++i) // rotate
            {
                transform.Rotate(new Vector3(0, 1, 0) * 3);
                yield return new WaitForSeconds(0.01f);
            }
        } else
        {
            for (int i = 0; i < 5; ++i) // rotate
            {
                transform.Rotate(new Vector3(0, 1, 0) * 3);
                yield return new WaitForSeconds(0.01f);
            }
            for (int i = 0; i < 5; ++i) // rotate
            {
                transform.Rotate(new Vector3(0, -1, 0) * 3);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
    IEnumerator RotateJump()
    {
        for (int i = 0; i < 8; ++i) // rotate
        {
            transform.Rotate(new Vector3(-0.5f, 0, 0) * 3);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 8; ++i) // restore
        {
            transform.Rotate(new Vector3(0.5f, 0, 0) * 3);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
