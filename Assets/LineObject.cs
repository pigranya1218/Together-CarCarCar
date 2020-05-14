using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineObject : MonoBehaviour
{
    bool needRespawn;
    void Start()
    {
        needRespawn = false;
    }

    void Update()
    {
        if(needRespawn)
        {
            needRespawn = false;
            Vector3 newPos = transform.position;
            newPos.x = -24;
            transform.position = newPos;
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea")) needRespawn = true;
    }
}
