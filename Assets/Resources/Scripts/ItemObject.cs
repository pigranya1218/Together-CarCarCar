using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int index;
    Rigidbody rigidbody;
    bool needActiveFalse;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (needActiveFalse)
        {
            gameObject.SetActive(false);
            needActiveFalse = false;
            StageManager.instance.EnqueItem(index);
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
            StageManager.instance.BoostModeOn();
        }
    }

    public void setField(int index)
    {
        this.index = index;
    }
}
