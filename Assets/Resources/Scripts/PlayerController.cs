using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;

    public int jumpPower;
    public float moveSpeedZ; // left, right move speed
    GameObject[] roads;  // use to computes moveLeft, moveRight
    public GameObject road0;
    public GameObject road1;
    public GameObject road2;
    public GameObject road3;
    public GameObject road4;

    int maxSpeed = 6;
    int currentPos; // current pos of player's car ([0, 4])
    int targetPos;
    int currentLife; // current life of player

    bool isGround;
    bool doJump;
    bool doMoveLeft;
    bool doMoveRight;
    bool isRestoring;

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

    public int getMaxSpeed()
    {
        return maxSpeed;
    }

    public bool getRestoring()
    {
        return this.isRestoring;
    }

    public void setRestoring(bool isRestoring)
    {
        this.isRestoring = isRestoring;
    }

    public void setTransparent(bool transparent)
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

}
