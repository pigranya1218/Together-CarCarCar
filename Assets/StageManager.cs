using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance = null;

    GameObject[] obstacleArray;  // obstacle cars
    GameObject[] treeArray; // trees
    GameObject[] itemArray; // items

    public GameObject treePrefab;
    public GameObject obstacle1Prefab;  // move to player obstacle, width 1 
    public GameObject obstacle2Prefab;  // stop obstacle, width 2
    public GameObject itemPrefab;

    public GameObject initRoad0;
    public GameObject initRoad1;
    public GameObject initRoad2;
    public GameObject initRoad3;
    public GameObject initRoad4;


    float currentSpeed = 0.01f;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        obstacleArray = new GameObject[20];
        treeArray = new GameObject[10]; // 0~4 : left, 5~9 : right
        itemArray = new GameObject[10];

        readStage(1);
        SpawnTrees();
    }
    
    void SpawnTrees()
    {
        for(int i = 0; i < 5; ++i)
        {
            treeArray[i] = Instantiate(treePrefab, new Vector3(-8 + (4.5f * i), 0.1f, 0.04f), Quaternion.identity);
        }
        for (int i = 5; i < 10; ++i)
        {
            treeArray[i] = Instantiate(treePrefab, new Vector3(-8 + (4.5f * (i - 5)), 0.1f, 11.9f), Quaternion.identity);
        }
    }

    void Update()
    {
        // move game objects() per frame
        for(int i = 0; i < 10; ++i)
        {
            moveToPlayer(treeArray[i]);
        }
    }

    private void moveToPlayer(GameObject gameObject)
    {
        Vector3 currentPos = gameObject.transform.position;
        currentPos.x += currentSpeed;
        gameObject.transform.position = currentPos;
    }

    public void readStage(int stageNum)
    {

    }
}
