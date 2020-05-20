using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : MonoBehaviour
{
    public int index;
    public int type;
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
            stageManager.EnqueObstacle(index, type);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea"))
        {
            needActiveFalse = true;
        } else if (other.gameObject.CompareTag("Player") && !playerController.GetRestoring())
        {
            Vector3 force = new Vector3(0, 15, Random.Range(-15f, 15f));
            rigidbody.AddForce(force, ForceMode.Impulse);
            rigidbody.AddTorque(new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f)) * 50);
            stageManager.ObstacleHit();
        }
    }

    public void setField(int index, int type)
    {
        this.index = index;
        this.type = type;
    }
}
