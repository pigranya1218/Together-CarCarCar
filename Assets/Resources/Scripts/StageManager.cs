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
    Queue<int> waitingItemIndex = new Queue<int>();

    GameObject[] treeArray; // trees
    GameObject[] lineArray; // road lines
    
    GameObject[] itemArray; // TODO: items
    GameObject endLine;
    GameObject[] explosion;

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
    public GameObject explosionEffectPrefab; // explosion effect prefab
    public GameObject boostPrefab; // boost item prefab

    public UISlider progressBar;
    public UILabel timeLabel;

    public enum OBJECT
    {
        TAXI = 1,
        BUS = 2,
        BOOST = 3
    }

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
    float startTime;
    float time;

    private Coroutine lastBoost;
    private Coroutine lastSpeedToMax;

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
        itemArray = new GameObject[5];

        treeArray = new GameObject[10]; // 0~4 : left, 5~9 : right
        itemArray = new GameObject[10];
        lineArray = new GameObject[20];
        explosion = new GameObject[11];

        roadArray = new GameObject[5];
        roadArray[0] = road0;
        roadArray[1] = road1;
        roadArray[2] = road2;
        roadArray[3] = road3;
        roadArray[4] = road4;

        speedUp = false;
        maxSpeed = PlayerController.instance.GetMaxSpeed();
        currentFrame = 0;

        LoadStage(2);
        SpawnObstacle();
        SpawnItem();
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
        
        for (int i = 0; i < 11; ++i)
        {
            explosion[i] = Instantiate(explosionEffectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            explosion[i].SetActive(false);
        }
    }

    void SpawnObstacle()
    {
        for(int i = 0; i < 15; ++i)
        {
            taxiArray[i] = Instantiate(taxiPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
            taxiArray[i].GetComponent<ObstacleObject>().setField(i, (int) OBJECT.TAXI);
            taxiArray[i].SetActive(false);
            
            busArray[i] = Instantiate(busPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            busArray[i].GetComponent<ObstacleObject>().setField(i, (int) OBJECT.BUS);
            busArray[i].SetActive(false);

            waitingTaxiIndex.Enqueue(i);
            waitingBusIndex.Enqueue(i);
        }
    }

    void SpawnItem()
    {
        for (int i = 0; i < 5; ++i)
        {
            itemArray[i] = Instantiate(boostPrefab, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
            itemArray[i].GetComponent<ItemObject>().setField(i);
            itemArray[i].SetActive(false);

            waitingItemIndex.Enqueue(i);
        }
    }


    void Update()
    {
        if (speedUp)  // speed up to Max speed
        {
            speedUp = false;
            lastSpeedToMax = StartCoroutine(SpeedToMax());
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
                if (taxiArray[i].activeSelf)
                {
                    if (taxiArray[i].GetComponent<ObstacleObject>().getCrash()) MoveToPlayer(taxiArray[i], 0);
                    else MoveToPlayer(taxiArray[i], 10);
                }
            }
            for (int i = 0; i < 15; ++i)
            {
                if (busArray[i].activeSelf) MoveToPlayer(busArray[i], 0);
            }
            for (int i = 0; i < 5; ++i)
            {
                if (itemArray[i].activeSelf) MoveToPlayer(itemArray[i], 0);
            }
            if (endLine.activeSelf) MoveToPlayer(endLine, 0);

            for (int i = 1; i < 11; ++i)
            {
                if (explosion[i].activeSelf) MoveToPlayer(explosion[i], 0);
            }
            
            time = Time.time - startTime;
            timeLabel.text = string.Format("{0:0.##}", time) + "s";
        }
    }

    private void MoveToPlayer(GameObject gameObject, int objectSpeed)
    {
        Vector3 currentPos = gameObject.transform.position;
        currentPos.x += (currentSpeed + objectSpeed) * Time.deltaTime;
        gameObject.transform.position = currentPos;
    }

    public void LoadStage(int stageNum)
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
        maxFrame = count;
        stageInfo = new int[maxFrame, 6]; // 0 : frame, 1 : pos
        for (int i = 0; i < maxFrame; ++i)
        {
            string[] info = lines[i].Split(delimeter); // 9 = \t, index = 0 : frame index, 1~5 : object info
            stageInfo[i, 0] = Convert.ToInt32(info[0]);
            for (int j = 1; j <= 5; ++j)
            {
                if (info[j] == "o1")
                {
                    stageInfo[i, j] = (int) OBJECT.TAXI;
                }
                else if (info[j] == "o2")
                {
                    stageInfo[i, j] = (int) OBJECT.BUS;
                }
                else if (info[j] == "b")
                {
                    stageInfo[i, j] = (int) OBJECT.BOOST;
                }
            }
        }
        PlayerController.instance.SetFinish(false);
        PlayerController.instance.Ready();
        currentFrameIndex = 0;
        isFinish = false;
        isMoreLoading = true;
        progressBar.value = 0;
        speedUp = true;
        time = 0;
        stopTime = false;
        startTime = Time.time;
        StartCoroutine(LoadingObstacles());
    }

    public void EnqueObstacle(int index, int type)
    {
        if(type == (int) OBJECT.TAXI)
        {
            waitingTaxiIndex.Enqueue(index);
        } else if(type == (int) OBJECT.BUS)
        {
            waitingBusIndex.Enqueue(index);
        }
    }

    public void EnqueItem(int index)
    {
        waitingItemIndex.Enqueue(index);
    }

    public void FinishStage()
    {
        progressBar.value = 1;
        stopTime = true;
        PlayerController.instance.ShowBoost(false);
        PlayerController.instance.SetFinish(true);
        StartCoroutine(SpeedToZero());
    }

    public int GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void ObstacleHitByPlayer()
    {
        PlayerController.instance.SetRestoring(true);
        PlayerController.instance.ShowBoost(false);
        if (lastSpeedToMax != null) StopCoroutine(lastSpeedToMax);
        if (lastBoost != null) StopCoroutine(lastBoost);
        StartCoroutine(ObstacleCollisionByPlayer());
    }
    public void ObstacleHitByObstacle(int index, int type)  // taxi + bus
    {
        if (type == (int)OBJECT.TAXI)
        {
            for(int i = 1; i < 11; ++i)
            {
                if(!explosion[i].activeSelf)
                {
                    explosion[i].transform.position = taxiArray[index].transform.position;
                    explosion[i].SetActive(true);
                    break;
                }
            }
        }
    }

    public void BoostModeOn()
    {
        if(lastSpeedToMax != null) StopCoroutine(lastSpeedToMax);
        if(lastBoost != null) StopCoroutine(lastBoost);
        lastBoost = StartCoroutine(SpeedToBoost());
    }

    IEnumerator LoadingObstacles()
    {
        // make obstacle per frame
        while(isMoreLoading)
        {
            for (int i = 0; i < currentSpeed * 10; ++i)
            {
                if (currentFrameIndex == maxFrame)
                {
                    endLine.transform.position = new Vector3(-30, 0.01f, 6);
                    endLine.SetActive(true);
                    isMoreLoading = false;
                    break;
                }
                int checkFrame = currentFrame + i;
                if (checkFrame == stageInfo[currentFrameIndex, 0])
                {
                    for (int j = 1; j <= 5; ++j)
                    {
                        if (stageInfo[currentFrameIndex, j] == (int) OBJECT.TAXI)
                        {
                            int waitingTaxi = waitingTaxiIndex.Dequeue();
                            taxiArray[waitingTaxi].transform.position = new Vector3(-23 - 0.01f * i, 0.01f, roadArray[j - 1].transform.position.z);
                            taxiArray[waitingTaxi].SetActive(true);

                        }
                        else if (stageInfo[currentFrameIndex, j] == (int) OBJECT.BUS)
                        {
                            int waitingBus = waitingBusIndex.Dequeue();
                            busArray[waitingBus].transform.position = new Vector3(-23 - 0.01f * i, 0.01f, roadArray[j - 1].transform.position.z);
                            busArray[waitingBus].SetActive(true);
                        }
                        else if (stageInfo[currentFrameIndex, j] == (int)OBJECT.BOOST)
                        {
                            int waitingItem = waitingItemIndex.Dequeue();
                            itemArray[waitingItem].transform.position = new Vector3(-23 - 0.01f * i, 0.01f, roadArray[j - 1].transform.position.z);
                            itemArray[waitingItem].SetActive(true);
                        }
                    }
                    currentFrameIndex++;
                }
            }
            currentFrame += currentSpeed * 10;
            progressBar.value = (float)currentFrame / ((float)stageInfo[maxFrame - 1, 0] + 500);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SpeedToMax()
    {
        PlayerController.instance.SetRestoring(false);
        while (currentSpeed != maxSpeed && !stopTime)
        {
            currentSpeed += (currentSpeed < maxSpeed)?1:-1;
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator SpeedToZero()
    {
        while(currentSpeed > 0)
        {
            currentSpeed -= 2;
            PlayerController.instance.Rotate(new Vector3(0, -1, 0) * 4);
            yield return new WaitForSeconds(0.05f);
        }
        if (currentSpeed < 0) currentSpeed = 0;
        isFinish = true;
    }
    IEnumerator SpeedToBoost()
    {
        PlayerController.instance.ShowBoost(true);
        while (currentSpeed < maxSpeed * 3 && !stopTime)
        {
            currentSpeed += 1;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2f);
        PlayerController.instance.ShowBoost(false);
        speedUp = true;
    }

    IEnumerator ObstacleCollisionByPlayer()
    {
        explosion[0].transform.position = new Vector3(PlayerController.instance.transform.position.x - 0.5f, PlayerController.instance.transform.position.y, PlayerController.instance.transform.position.z);
        explosion[0].SetActive(true);
        currentSpeed = 2;
        for(int i = 0; i < 10; ++i)
        {
            if (i % 2 == 0) PlayerController.instance.SetTransparent(true);
            else PlayerController.instance.SetTransparent(false); ;

            yield return new WaitForSeconds(0.2f);
        }
        explosion[0].SetActive(false);
        speedUp = true;
    }
}
