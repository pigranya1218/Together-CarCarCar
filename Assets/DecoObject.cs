using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoObject : MonoBehaviour
{
    bool isGround;
    void Start()
    {
        isGround = true;
    }

    void Update()
    {
        if (!isGround)
        {
            isGround = true;
            Vector3 newPos = transform.position;
            newPos.x = -8.5f;
            newPos.y = 0.15f;
            transform.position = newPos;
            
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter");
        if (other.gameObject.CompareTag("MissingArea")) isGround = false;
    }
}
