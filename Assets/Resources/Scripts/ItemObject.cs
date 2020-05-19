using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int index;
    StageManager stageManager;
    PlayerController playerController;
    Rigidbody rigidbody;
    bool needActiveFalse;

    void Start()
    {
        stageManager = StageManager.instance;
        playerController = PlayerController.instance;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (needActiveFalse)
        {
            gameObject.SetActive(false);
            needActiveFalse = false;
            stageManager.EnqueItem(index);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea"))
        {
            needActiveFalse = true;
        } else if (other.gameObject.CompareTag("Player"))
        {
            needActiveFalse = true;
            stageManager.BoostModeOn();
        }
    }

    public void setField(int index)
    {
        this.index = index;
    }
}
