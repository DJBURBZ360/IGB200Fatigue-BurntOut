﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Employee : MonoBehaviour
{
    public enum FatigueTypes { Dizziness, Headache, Sleepiness };

    #region Variables
    [SerializeField]
    [Header("Generate Random Fatigue Time Between (in seconds)")]
    private float[] randomNumRange = new float[2];

    private int currentFatigueLevel = 0;
    [SerializeField] private int maxFatigueLevel = 0;

    private float currentFatigueRate = 0,
                  currentFatigueDelay = 0;
    [SerializeField] private FatigueTypes currentFatigueType;

    private bool isChecking = true;

    private GameObject fatigueUI;
    private Slider fatigueSlider;
    private Text fatigueText;
    private Image fillColor;

    private GameManager gameManager;
    private CarManager car;

    //[SerializeField] private Sprite[] driverStates = new Sprite [4];
    [SerializeField] private int[][] test;
    private SpriteRenderer renderer;
    [SerializeField] Sprite defaultDriverState;
    [SerializeField] Sprites2DArray[] driverStates;//row = fatigue type
                                                   //col = fatigue stage
    #endregion

    #region Accessors
    public CarManager Car
    {
        get { return car; }
    }

    public GameObject FatigueUI
    {
        get { return fatigueUI; }
        set { fatigueUI = value; }
    }

    public int CurrentFatigueLevel
    {
        get { return currentFatigueLevel; }
    }
    #endregion

    #region Methods
    private void IncreaseFatigueLevel()
    {
        //countdown then incease fatigue level
        if (currentFatigueDelay < Time.time)
        {
            //randomly generate a number
            currentFatigueRate = Random.Range(randomNumRange[0], randomNumRange[1]);
            currentFatigueDelay = currentFatigueRate + Time.time;
            
            //limits the current fatigue level
            int temp = ++currentFatigueLevel;
            if (temp > maxFatigueLevel)
            {
                temp = maxFatigueLevel;
            }
            currentFatigueLevel = temp;
            
            //reset slider fill then change color
            fatigueSlider.value = 0;
            fatigueSlider.maxValue = currentFatigueRate;
            fillColor.color = gameManager.fatigueLevelColors[currentFatigueLevel];
        }
    }

    private void DecreaseFatigueLevel()
    {
        //decrease fatigue level when given the appropriate snack
        currentFatigueLevel = currentFatigueLevel < 0 ? 0 : --currentFatigueLevel;
    }

    private void CheckForSnack(Collider2D collision)
    {
        Snack givenSnack = collision.GetComponent<Snack>();
        switch (currentFatigueLevel)
        {
            case 1:
                if (givenSnack.SnackLevel == Snack.SnackLevels.level1)
                {
                    ResetFatigueLevel();
                }
                else
                {
                    if (givenSnack.SnackLevel > Snack.SnackLevels.level1)
                    {
                        //no effects given
                    }
                }
                break;

            case 2:
                if (givenSnack.SnackLevel == Snack.SnackLevels.level2)
                {
                    ResetFatigueLevel();
                }
                else
                {
                    if (givenSnack.SnackLevel < Snack.SnackLevels.level2)
                    {
                        DecreaseFatigueGauge();
                    }
                    else if (givenSnack.SnackLevel > Snack.SnackLevels.level2)
                    {
                        //no effects given 
                    }
                }
                break;

            case 3:
                if (givenSnack.SnackLevel == Snack.SnackLevels.level3)
                {
                    //Employee is sent home
                    ResetFatigueLevel();
                    car.ChangeDriver();
                }
                else
                {
                    if (givenSnack.SnackLevel < Snack.SnackLevels.level3)
                    {
                        DecreaseFatigueGauge();
                    }
                }
                break;

            default:Debug.Log(this.gameObject.name + "'s fatigue level: " + currentFatigueLevel);
                break;
        }
    }

    /// <summary>
    /// Resets fatigue level and fatigue timer.
    /// </summary>
    public void ResetFatigueLevel()
    {
        GenerateRandomFatigue();
        currentFatigueLevel = 0;
        currentFatigueRate = Random.Range(randomNumRange[0], randomNumRange[1]);
        currentFatigueDelay = currentFatigueRate + Time.time;
        fillColor.color = gameManager.fatigueLevelColors[0];
    }

    /// <summary>
    /// -25% from the current gauge.
    /// </summary>
    private void DecreaseFatigueGauge()
    {
        
        float temp = currentFatigueDelay - Time.time < 0 ? 0 : currentFatigueDelay - Time.time;
        temp = (temp + (currentFatigueRate * 0.25f)) + Time.time;
        currentFatigueDelay = temp;
    }

    private void GenerateRandomFatigue()
    {
        int num = Random.Range(0, gameManager.AvailableFatigueTypes.Length);
        currentFatigueType = gameManager.AvailableFatigueTypes[num];
    }

    /// <summary>
    /// Do fail event when reached fatigue level 4.
    /// </summary>
    private void CheckStatus()
    {
        //quick switch to prevent unecesarry events
        if (car.IsParked)
        {
            isChecking = true;

            if (currentFatigueLevel >= 4)
                gameManager.IsPlaying = false;
        }
        else
        {
            //naive delayed checking
            if (isChecking)
            {
                //do speed boost
                //resets timer only
                if (currentFatigueLevel == 0)
                    Player.instance.ApplySpeedBoost();

                //if reached fatigue level 4, do fail event
                if (currentFatigueLevel >= 3)
                    gameManager.IsPlaying = false;
            }
            isChecking = false;
        }
    }

    /// <summary>
    /// Swaps out into a new sprite image that corresponds to the current fatigue level.
    /// </summary>
    private void ChangeState()
    {
        int row = (int)currentFatigueType;

        if (currentFatigueLevel == 0 &&
            defaultDriverState != null)
            renderer.sprite = defaultDriverState;

        else if (currentFatigueLevel > 0 && 
                 currentFatigueLevel < maxFatigueLevel &&
                 driverStates[row].spritesArray[currentFatigueLevel - 1] != null)
            renderer.sprite = driverStates[row].spritesArray[currentFatigueLevel - 1];
    }

    private void UpdateTimerUI()
    {
        fatigueSlider.value = currentFatigueRate - (currentFatigueDelay - Time.time);

        if (currentFatigueLevel < maxFatigueLevel)
            fatigueText.text = string.Format("Fatigue Level: {0}", currentFatigueLevel);
        else
            fatigueText.text = "Totally Fatigued!!!";
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManager>();
        renderer = this.gameObject.GetComponent<SpriteRenderer>();
        car = this.gameObject.transform.parent.GetComponent<CarManager>();

        //UI setup
        fatigueSlider = fatigueUI.transform.GetChild(0).GetComponent<Slider>();
        fatigueText = fatigueUI.transform.GetChild(1).GetComponent<Text>();
        fillColor = fatigueSlider.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        fillColor.color = gameManager.fatigueLevelColors[currentFatigueLevel];

        //initialize fatigue timer
        currentFatigueRate = Random.Range(randomNumRange[0], randomNumRange[1]);
        currentFatigueDelay = currentFatigueRate + Time.time;
        fatigueSlider.maxValue = currentFatigueRate;

        GenerateRandomFatigue();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimerUI();

        //increase only when below max fatigue level
        if (currentFatigueLevel < maxFatigueLevel)
        {
            IncreaseFatigueLevel();
        }
        CheckStatus();
        ChangeState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //snacks will be prefabs with snack script attached to ID snack level
        //instanciate snack when clicked on snack icon from UI
        if (collision.GetComponent<Snack>() != null)
        {
            if (!collision.GetComponent<Snack>().IsDragged)
            {
                CheckForSnack(collision);
                Player.instance.HasSnack = false;
                Snack.numInstance--;
                Destroy(collision.gameObject);
            }
        }
    }
}
