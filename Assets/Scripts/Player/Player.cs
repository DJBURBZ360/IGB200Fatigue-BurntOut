﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    #region Variables
    public static Player instance;

    [SerializeField] private Vector3 snackOffset;
    [SerializeField] private float normalSpeed = 20f;
    [SerializeField] private int speedBoostTime = 5;
    [SerializeField] private float speedBoostSpeed = 50f;
    [SerializeField] private Vector2[] boundaries =  new Vector2[2]; //0 - min, 1 - max
    [SerializeField] private GameObject speedBoostUI;
    [SerializeField] private GameObject sendHomeUI;
    [SerializeField] private GameObject worldSpaceCanvas;

    private float interpolation = 0;
    private float moveSpeed = 0;
    private float currentBoostTime = 0;
    
    private bool hasItem = false;
    private bool hasSpeedBoost = false;
    private bool enableMovement = true;

    private Vector2[] travelPoints = new Vector2 [2];
    private Employee currentTarget;
    private Animator animator;
    private Text speedBoostTimeText;
    private GameObject currentItem;
    private GameManager gameManager;
    #endregion

    #region Accessors
    public Vector3 ItemOffset
    {
        get { return snackOffset; }
    }

    public bool HasItem
    {
        get { return hasItem; }
        set { hasItem = value; }
    }

    public bool EnableMovement
    {
        get { return enableMovement; }
        set { enableMovement = value; }
    }

    public bool HasSpeedBoost
    {
        get { return hasSpeedBoost; }
    }

    public GameObject CurrentItem
    {
        get { return currentItem; }
        set { currentItem = value; }
    }
    #endregion

    #region Methods
    private void SendEmployeeHome()
    {
        if (Input.GetButtonDown("Interact") &&
            currentTarget != null)
        {
            currentTarget.SendHome();
        }
    }

    private void DoMoveAnimation()
    {
        if (enableMovement)
        {
            //left & right
            if (Input.GetAxis("Horizontal") > 0 ||
                Input.GetAxis("Horizontal") < 0)
            {
                animator.SetBool("Move_Horizontal", true);
                animator.SetBool("Move_Up", false);
                animator.SetBool("Move_Down", false);
            }
            else
            {
                animator.SetBool("Move_Horizontal", false);
                
                if (Input.GetAxis("Vertical") > 0) //up
                {
                    animator.SetBool("Move_Up", true);
                    animator.SetBool("Move_Down", false);
                }
                else if (Input.GetAxis("Vertical") < 0) //down
                {
                    animator.SetBool("Move_Up", false);
                    animator.SetBool("Move_Down", true);
                }
                else
                {
                    animator.SetBool("Move_Up", false);
                    animator.SetBool("Move_Down", false);
                }
            }                
        }
        else
        {
            animator.SetBool("Move_Horizontal", false);
            animator.SetBool("Move_Up", false);
            animator.SetBool("Move_Down", false);
        }
    }

    /// <summary>
    /// Flips the character facing the appropriate direction
    /// </summary>
    private void Flip()
    {
        if (Input.GetButton("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") < 0) //left
            {
                if (transform.localScale.x > 0)
                {
                    transform.localScale *= new Vector2(-1, 1);

                    if (worldSpaceCanvas != null)
                    {
                        worldSpaceCanvas.transform.localScale *= new Vector2(-1, 1);
                        if (currentItem != null) currentItem.transform.localScale *= new Vector2(-1, 1);
                    }
                }
            }
            else //right
            {
                if (transform.localScale.x < 0)
                {
                    transform.localScale *= new Vector2(-1, 1);

                    if (worldSpaceCanvas != null)
                    {
                        worldSpaceCanvas.transform.localScale *= new Vector2(-1, 1);
                        if (currentItem != null) currentItem.transform.localScale *= new Vector2(-1, 1);
                    }
                }
            }
        }        
    }

    private void Move()
    {
        if (Input.GetAxis("Horizontal") < 0) //left
        {
            transform.position -= gameObject.transform.right * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Horizontal") > 0) //right
        {
            transform.position += gameObject.transform.right * moveSpeed * Time.deltaTime;
        }

        if (Input.GetAxis("Vertical") < 0) //down
        {
            transform.position -= gameObject.transform.up * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Vertical") > 0) //up
        {
            transform.position += gameObject.transform.up * moveSpeed * Time.deltaTime;
        }
    }
    
    private void LimitMovement()
    {
        //horizontal boundary
        if (transform.position.x < boundaries[0].x)
        {
            transform.position = new Vector2(boundaries[0].x,
                                             transform.position.y);
        }
        else if (transform.position.x > boundaries[1].x)
        {
            transform.position = new Vector2(boundaries[1].x,
                                             transform.position.y);
        }

        //vertical boundary
        if (transform.position.y < boundaries[0].y)
        {
            transform.position = new Vector2(transform.position.x,
                                             boundaries[0].y);
        }
        else if (transform.position.y > boundaries[1].y)
        {
            transform.position = new Vector2(transform.position.x,
                                             boundaries[1].y);
        }
    }

    private void UpdateSpeedBoostUI()
    {
        speedBoostTimeText.text = string.Format("{0:0.00}s", currentBoostTime - Time.time);
    }

    private void CheckSpeedBoostState()
    {
        if (currentBoostTime < Time.time)
        {
            ResetSpeedBoost();
        }
    }
    #endregion

    #region Public Methods
    public void ApplySpeedBoost()
    {
        moveSpeed = speedBoostSpeed;
        hasSpeedBoost = true;
        speedBoostUI.SetActive(true);
        currentBoostTime = speedBoostTime + Time.time;
    }

    public void ResetSpeedBoost()
    {
        moveSpeed = normalSpeed;
        hasSpeedBoost = false;
        speedBoostUI.SetActive(false);
    }

    public void SimulateDropItem()
    {
        currentItem.GetComponent<Item>().ForceDropItem();
    }

    public void ForceFlipItem()
    {
        currentItem.transform.localScale *= new Vector2(-1, 1);
        currentItem.transform.localPosition *= new Vector2(-1, 1);
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        instance = this.gameObject.GetComponent<Player>();
        animator = this.gameObject.GetComponent<Animator>();
        travelPoints[0] = transform.position;
        moveSpeed = normalSpeed;
        speedBoostTimeText = speedBoostUI.transform.GetChild(0).GetComponent<Text>();
        gameManager = GameObject.FindWithTag("Managers").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsTutorialActive)
        {
            if (hasSpeedBoost)
            {
                UpdateSpeedBoostUI();
                animator.speed = 2;
            }
            else
            {
                animator.speed = 1;
            }
            CheckSpeedBoostState();
        }

        if (enableMovement)
        {
            Move();
            Flip();
        }

        DoMoveAnimation();
        LimitMovement();        
        SendEmployeeHome();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (currentTarget == null &&
            collision.gameObject.GetComponent<Employee>() != null)
        {
            Employee employee = collision.gameObject.GetComponent<Employee>();

            if (employee.CurrentFatigueLevel > 2)
            {
                sendHomeUI.SetActive(true);
                currentTarget = employee;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Employee>() != null)
        {
            sendHomeUI.SetActive(false);
            currentTarget = null;
        }
    }
}
