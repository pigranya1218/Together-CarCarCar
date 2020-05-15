using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoObject : MonoBehaviour
{
    bool needRespawn;
    void Start()
    {
        needRespawn = false;
    }

    void Update()
    {
        if (needRespawn)
        {
            needRespawn = false;
            Vector3 newPos = transform.position;
            newPos.x = -23f;
            newPos.y = 0.15f;
            transform.position = newPos;
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea")) needRespawn = true;
    }
}
