using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    public static StageManager instance = null;

    GameObject[] taxiArray;  // obstacle taxi
    GameObject[] busArray;  // obstacle bus
    Queue<int> waitingTaxiIndex = new Queue<int>();  // for avoiding 3-for
    Queue<int> waitingBusIndex = new Queue<int>();

    GameObject[] treeArray; // trees
    GameObject[] lineArray; // road lines
    
    GameObject[] itemArray; // TODO: items
    GameObject endLine;
    GameObject explosion;

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
    public GameObject explosionPrefab; // explosion effect prefab
    
    public GameObject itemPrefab; // TODO: item

    public UISlider progressBar;
    public UILabel timeLabel;

    public enum Obstacle
    {
        taxi = 1,
        bus = 2
    }

    PlayerController playerController;

    int maxSpeed;
    int currentSpeed = 0;
    int currentFrame;  // current frame of stage
    int currentFrameIndex;
    int maxFrame;  // max frame of stage
    bool isMoreLoading;
    bool speedUp;
    bool isFinish;
    bool stopTime;
    string stagePath = "Assets/Resources/";
    int[,] stageInfo;
    float time;


    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerController = PlayerController.instance;

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

        speedUp = false;
        maxSpeed = playerController.getMaxSpeed();
        currentFrame = 0;

        ReadStage(1);
        SpawnObstacle();
        SpawnDecos();
    }
    
    void SpawnDecos()
    {
        for (int i = 0; i < 5; ++i)
        {
            treeArray[i] = Instantiate(treePrefab, new Vector3(-23 + (8 * i), 0.1f, 0.04f), Quaternion.identity);
        }
        for (int i = 5; i < 10; ++i)
        {
            treeArray[i] = Instantiate(treePrefab, new Vector3(-23 + (8 * (i - 5)), 0.1f, 11.9f), Quaternion.identity);
        }
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
        explosion = Instantiate(explosionPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        explosion.SetActive(false);
    }

    void SpawnObstacle()
    {
        for(int i = 0; i < 15; ++i)
        {
            taxiArray[i] = Instantiate(taxiPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
            taxiArray[i].GetComponent<ObstacleObject>().setField(i, (int)Obstacle.taxi);
            taxiArray[i].SetActive(false);
            
            busArray[i] = Instantiate(busPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            busArray[i].GetComponent<ObstacleObject>().setField(i, (int)Obstacle.bus);
            busArray[i].SetActive(false);

            waitingTaxiIndex.Enqueue(i);
            waitingBusIndex.Enqueue(i);
        }
    }

    void Update()
    {
        if (speedUp)  // speed up to Max speed
        {
            speedUp = false;
            StartCoroutine(SpeedToMax());
        }
        if (isMoreLoading)
        {
            // make obstacle per frame
            for (int i = 0; i < currentSpeed; ++i)
            {
                if(currentFrameIndex == maxFrame)
                {
                    endLine.transform.position = new Vector3(-30, 0.01f, 6);
                    endLine.SetActive(true);
                    isMoreLoading = false;
                    progressBar.value = 1;
                    break;
                }
                int checkFrame = currentFrame + i;
                if(checkFrame == stageInfo[currentFrameIndex, 0])
                {
                    for (int j = 1; j <= 5; ++j)
                    {
                        if (stageInfo[currentFrameIndex, j] == (int) Obstacle.taxi)
                        {
                            int waitingTaxi = waitingTaxiIndex.Dequeue();
                            taxiArray[waitingTaxi].transform.position = new Vector3(-23 - 0.01f * i, 0.01f, roadArray[j - 1].transform.position.z);
                            taxiArray[waitingTaxi].SetActive(true);
                            
                        } else if (stageInfo[currentFrameIndex, j] == (int) Obstacle.bus)
                        {
                            int waitingBus = waitingBusIndex.Dequeue();
                            busArray[waitingBus].transform.position = new Vector3(-23 - 0.01f * i, 0.01f, roadArray[j - 1].transform.position.z);
                            busArray[waitingBus].SetActive(true);
                        }
                    }
                    currentFrameIndex++;
                }
            }
            currentFrame += currentSpeed;
            progressBar.value = (float)currentFrame / (float) stageInfo[maxFrame - 1, 0];
        }

        // move game objects() per frame
        if(!isFinish)
        {
            for (int i = 0; i < 10; ++i)
            {
                if (treeArray[i].activeSelf) MoveToPlayer(treeArray[i], 0);
            }
            for (int i = 0; i < 20; ++i)
            {
                if (lineArray[i].activeSelf) MoveToPlayer(lineArray[i], 0);
            }
            for (int i = 0; i < 15; ++i)
            {
                if (taxiArray[i].activeSelf) MoveToPlayer(taxiArray[i], 4);
            }
            for (int i = 0; i < 15; ++i)
            {
                if (busArray[i].activeSelf) MoveToPlayer(busArray[i], 0);
            }
            if (endLine.activeSelf) MoveToPlayer(endLine, 0);
        }
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
        maxFrame = --count;
        stageInfo = new int[maxFrame, 6]; // 0 : frame, 1 : pos
        for (int i = 0; i < maxFrame; ++i)
        {
            string[] info = lines[i].Split(delimeter); // 9 = \t, index = 0 : frame index, 1~5 : object info
            stageInfo[i, 0] = Convert.ToInt32(info[0]);
            for (int j = 1; j <= 5; ++j)
            {
                if (info[j] == "o1")
                {
                    stageInfo[i, j] = (int) Obstacle.taxi;
                }
                if (info[j] == "o2")
                {
                    stageInfo[i, j] = (int) Obstacle.bus;
                }
            }
        }
        currentFrameIndex = 0;
        isFinish = false;
        isMoreLoading = true;
        progressBar.value = 0;
        speedUp = true;
        time = 0;
        stopTime = false;
        StartCoroutine(StartTime());
    }

    public void EnqueObstacle(int index, int type)
    {
        if(type == (int) Obstacle.taxi)
        {
            waitingTaxiIndex.Enqueue(index);
        } else if(type == (int) Obstacle.bus)
        {
            waitingBusIndex.Enqueue(index);
        }
    }

    public void FinishStage()
    {
        stopTime = true;
        StartCoroutine(SpeedToZero());
    }

    public void ObstacleHit()
    {
        playerController.setRestoring(true);
        StartCoroutine(ObstacleCollision());
    }
    IEnumerator StartTime()
    {
        while (!stopTime)
        {
            time += 0.01f;
            timeLabel.text = string.Format("{0:0.##}", time) + "s";
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator SpeedToMax()
    {
        playerController.setRestoring(false);
        while (currentSpeed != maxSpeed)
        {
            currentSpeed += 1;
            yield return new WaitForSeconds(0.3f);
        }
    }
    IEnumerator SpeedToZero()
    {
        while(currentSpeed != 0)
        {
            currentSpeed -= 1;
            yield return new WaitForSeconds(0.3f);
        }
        isFinish = true;
    }

    IEnumerator ObstacleCollision()
    {
        explosion.transform.position = new Vector3(playerController.transform.position.x - 0.5f, playerController.transform.position.y, playerController.transform.position.z);
        explosion.SetActive(true);
        currentSpeed = 2;
        for(int i = 0; i < 10; ++i)
        {
            if (i % 2 == 0) playerController.setTransparent(true);
            else playerController.setTransparent(false); ;

            yield return new WaitForSeconds(0.2f);
        }
        explosion.SetActive(false);
        speedUp = true;
    }


}
