using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLineObject : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea"))
        {
            Debug.Log("Stage is End!");            
            gameObject.SetActive(false);
        }
    }
}
