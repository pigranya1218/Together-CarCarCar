using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : MonoBehaviour
{
    public int index;
    public int type;
    StageManager stageManager;
    PlayerController playerController;
    bool needActiveFalse;

    void Start()
    {
        stageManager = StageManager.instance;
        playerController = PlayerController.instance;
    }

    void Update()
    {
        if (needActiveFalse)
        {
            gameObject.SetActive(false);
            needActiveFalse = false;
            stageManager.EnqueObstacle(index, type);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea"))
        {
            needActiveFalse = true;
        } else if (other.gameObject.CompareTag("Player") && !playerController.getRestoring())
        {
            stageManager.ObstacleHit();
        }
    }

    public void setField(int index, int type)
    {
        this.index = index;
        this.type = type;
    }
}
