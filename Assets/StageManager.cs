using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance = null;

    GameObject[] obstacleArray;  // obstacle cars
    GameObject[] treeArray; // trees
    GameObject[] itemArray; // items
    GameObject[] lineArray; // road lines

    public GameObject treePrefab;
    public GameObject linePrefab;
    public GameObject obstacle1Prefab;  // move to player obstacle, width 1 
    public GameObject obstacle2Prefab;  // stop obstacle, width 2
    public GameObject itemPrefab;

    float currentSpeed = 0.04f;

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
        lineArray = new GameObject[20];

        ReadStage(1);
        SpawnTrees();
        SpawnLines();
    }
    
    void SpawnTrees()
    {
        for(int i = 0; i < 5; ++i)
        {
            treeArray[i] = Instantiate(treePrefab, new Vector3(-23 + (8 * i), 0.1f, 0.04f), Quaternion.identity);
        }
        for (int i = 5; i < 10; ++i)
        {
            treeArray[i] = Instantiate(treePrefab, new Vector3(-23 + (8 * (i - 5)), 0.1f, 11.9f), Quaternion.identity);
        }
    }

    void SpawnLines()
    {
        for (int i = 0; i < 5; ++i)
        {
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * i), 0.1f, 2.96f), Quaternion.identity);
        }
        for (int i = 5; i < 10; ++i)
        {
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * (i - 5)), 0.1f, 4.95f), Quaternion.identity);
        }
        for (int i = 10; i < 15; ++i)
        {
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * (i - 10)), 0.1f, 6.88f), Quaternion.identity);
        }
        for (int i = 15; i < 20; ++i)
        {
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * (i - 15)), 0.1f, 8.86f), Quaternion.identity);
        }
    }

    void Update()
    {
        // move game objects() per frame
        for(int i = 0; i < 10; ++i)
        {
            MoveToPlayer(treeArray[i]);
        }
        for(int i = 0; i < 20; ++i)
        {
            MoveToPlayer(lineArray[i]);
        }

    }

    private void MoveToPlayer(GameObject gameObject)
    {
        Vector3 currentPos = gameObject.transform.position;
        currentPos.x += currentSpeed;
        gameObject.transform.position = currentPos;
    }

    public void ReadStage(int stageNum)
    {

    }
}
