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
            StageManager.instance.FinishStage();
            gameObject.SetActive(false);
        }
    }
}
