using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLineObject : MonoBehaviour
{
    StageManager stageManager;
    void Start()
    {
        stageManager = StageManager.instance;
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea"))
        {
            stageManager.FinishStage();
            gameObject.SetActive(false);
        }
    }
}
