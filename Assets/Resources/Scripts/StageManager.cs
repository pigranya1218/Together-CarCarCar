using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public static StageManager instance = null;

    GameObject[] taxiArray;  // obstacle taxi
    GameObject[] busArray;  // obstacle bus

    GameObject[] treeArray; // trees
    GameObject[] lineArray; // road lines
    
    GameObject[] itemArray; // TODO: items
    GameObject endLine;


    public GameObject road0; // using for set obstacle position
    public GameObject road1;
    public GameObject road2;
    public GameObject road3;
    public GameObject road4;
    GameObject[] roadArray;

    
    public GameObject treePrefab;
    public GameObject linePrefab;
    public GameObject taxiPrefab;  // move to player obstacle, width 1 
    public GameObject busPrefab;  // stop obstacle, width 2
    public GameObject endLinePrefab; // finish line
    
    public GameObject itemPrefab; // TODO: item

    public enum obstacle
    {
        taxi = 1,
        bus = 2
    }

    int currentSpeed = 4;
    int currentFrame;  // current frame of stage
    int maxFrame;  // max frame of stage
    bool isPlaying;
    string stagePath = "Assets/Resources/";
    int[,] stageInfo;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        taxiArray = new GameObject[15];
        busArray = new GameObject[15];

        treeArray = new GameObject[10]; // 0~4 : left, 5~9 : right
        itemArray = new GameObject[10];
        lineArray = new GameObject[20];

        roadArray = new GameObject[5];
        roadArray[0] = road0;
        roadArray[1] = road1;
        roadArray[2] = road2;
        roadArray[3] = road3;
        roadArray[4] = road4;

        currentFrame = 0;

        ReadStage(1);
        SpawnObstacle();
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
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * i), 0.01f, 2.96f), Quaternion.identity);
        }
        for (int i = 5; i < 10; ++i)
        {
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * (i - 5)), 0.01f, 4.95f), Quaternion.identity);
        }
        for (int i = 10; i < 15; ++i)
        {
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * (i - 10)), 0.01f, 6.88f), Quaternion.identity);
        }
        for (int i = 15; i < 20; ++i)
        {
            lineArray[i] = Instantiate(linePrefab, new Vector3(-20f + (7.5f * (i - 15)), 0.01f, 8.86f), Quaternion.identity);
        }
        endLine = Instantiate(endLinePrefab, new Vector3(-20f, 0.01f, 8.86f), Quaternion.Euler(new Vector3(0, 90, 0)));
        endLine.SetActive(false);
    }

    void SpawnObstacle()
    {
        for(int i = 0; i < 15; ++i)
        {
            taxiArray[i] = Instantiate(taxiPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
            taxiArray[i].SetActive(false);
            
            busArray[i] = Instantiate(busPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            busArray[i].SetActive(false);
        }
    }

    void Update()
    {
        if (isPlaying)
        {
            // make obstacle per frame
            for (int i = 0; i < currentSpeed; ++i)
            {
                int checkFrame = currentFrame + i;
                if (checkFrame == (maxFrame - 1)) // end of this stage
                {
                    endLine.transform.position = new Vector3(-30, 0.01f, 6);
                    endLine.SetActive(true);
                    isPlaying = false;
                    break;
                }
                for (int j = 0; j < 5; ++j)
                {
                    if (stageInfo[checkFrame, j] == (int)obstacle.taxi)
                    {
                        for (int k = 0; k < 15; ++k)
                        {
                            if (!taxiArray[k].activeSelf)
                            {
                                taxiArray[k].transform.position = new Vector3(-23 - 0.01f * i, 0.01f, roadArray[j].transform.position.z);
                                taxiArray[k].SetActive(true);
                                break;
                            }
                        }
                    }
                    if (stageInfo[checkFrame, j] == (int)obstacle.bus)
                    {
                        for (int k = 0; k < 15; ++k)
                        {
                            if (!busArray[k].activeSelf)
                            {
                                busArray[k].transform.position = new Vector3(-23 - 0.01f * i, 0.01f, roadArray[j].transform.position.z);
                                busArray[k].SetActive(true);
                                break;
                            }
                        }
                    }
                }
            }
            currentFrame += currentSpeed;
        }

        // move game objects() per frame
        for (int i = 0; i < 10; ++i)
        {
            if(treeArray[i].activeSelf) MoveToPlayer(treeArray[i], 0);
        }
        for(int i = 0; i < 20; ++i)
        {
            if(lineArray[i].activeSelf) MoveToPlayer(lineArray[i], 0);
        }
        for(int i = 0; i < 15; ++i)
        {
            if(taxiArray[i].activeSelf) MoveToPlayer(taxiArray[i], 4);
        }
        for (int i = 0; i < 15; ++i)
        {
            if (busArray[i].activeSelf) MoveToPlayer(busArray[i], 0);
        }
        if (endLine.activeSelf) MoveToPlayer(endLine, 0);

    }

    private void MoveToPlayer(GameObject gameObject, int objectSpeed)
    {
        Vector3 currentPos = gameObject.transform.position;
        currentPos.x += (currentSpeed + objectSpeed) * 0.01f;
        gameObject.transform.position = currentPos;
    }

    public void ReadStage(int stageNum)
    {
        string stage = stageNum.ToString();
        AssetDatabase.ImportAsset(stagePath + stage + ".txt");
        TextAsset txt = Resources.Load(stage) as TextAsset;
        string[] lines = new string[50];
        int count = 0;
        using (StringReader reader = new StringReader(txt.text))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                lines[count++] = line;
            }
        }
        char delimeter = Convert.ToChar(9); // 9 = \t
        maxFrame = Convert.ToInt32(lines[--count].Split(delimeter)[0]) + 10; // end of stage frame
        stageInfo = new int[maxFrame, 5]; // 0 : frame, 1 : pos
        for (int i = 0; i <= count; ++i)
        {
            string[] info = lines[i].Split(delimeter); // 9 = \t, index = 0 : frame index, 1~5 : object info
            int frameIndex = Convert.ToInt32(info[0]);
            for (int j = 0; j < 5; ++j)
            {
                if (info[j + 1] == "o1")
                {
                    stageInfo[frameIndex, j] = (int)obstacle.taxi;
                }
                if (info[j + 1] == "o2")
                {
                    stageInfo[frameIndex, j] = (int)obstacle.bus;
                }
            }
        }
        isPlaying = true;
    }
}
