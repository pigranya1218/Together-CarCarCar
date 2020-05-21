using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObject : MonoBehaviour
{
    public int index;
    public int type;
    public bool isCrash;

    Rigidbody rigidbody;
    bool needActiveFalse;
    

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        isCrash = false;
    }

    void Update()
    {
        if (needActiveFalse)
        {
            gameObject.SetActive(false);
            needActiveFalse = false;
            isCrash = false;
            StageManager.instance.EnqueObstacle(index, type);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MissingArea"))
        {
            needActiveFalse = true;
        } else if (other.gameObject.CompareTag("Player") && !PlayerController.instance.GetRestoring())
        {
            Vector3 force = new Vector3(0, 15, Random.Range(-15f, 15f));
            rigidbody.AddForce(force, ForceMode.Impulse);
            rigidbody.AddTorque(new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f)) * 50);
            StageManager.instance.ObstacleHitByPlayer();
        } else if (other.gameObject.CompareTag("Obstacle"))
        {
            isCrash = true;
            StageManager.instance.ObstacleHitByObstacle(index, type);
        }
    }

    public void setField(int index, int type)
    {
        this.index = index;
        this.type = type;
    }

    public bool getCrash()
    {
        return isCrash;
    }
}
