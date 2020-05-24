using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject stagePanel;
    public GameObject startButton;
    public GameObject myCarButton;
    public GameObject optionButton;
    public int stageNum;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnClickStart()
    {
        stagePanel.SetActive(true);
    }

    public void OnClickMyCar()
    {

    }

    public void onClickOption()
    {

    }

    public void OnClickStage(UILabel stageLabel)
    {
        stageNum = Convert.ToInt32(stageLabel.text);
        SceneManager.LoadScene("stage");
    }
    
}
