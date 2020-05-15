using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : MonoBehaviour
{
    bool needActiveFalse;

    void Update()
    {
        if (needActiveFalse)
        {
            gameObject.SetActive(false);
            needActiveFalse = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea"))
        {
            Debug.Log("enter missing area");
            needActiveFalse = true;
        }
    }
}
