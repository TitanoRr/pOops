﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPun
{
    [SerializeField] private GameObject poop; //the poop prefab to be pooped!
    [SerializeField] private GameObject playerCanvas;

    [SerializeField] private Text playerNameText;

    [SerializeField] private Transform poopSpawner; //the poop spawner transform the player has

    [SerializeField] private PhysicsMaterial2D playerPhysMat; 

    [SerializeField] private int poopSpeed = 10; //the player's move speed
    [SerializeField] private int rotSpeed = 10; //the player's rotation speed

    [SerializeField] private Quaternion canvasInitRot; //initial canvas rotation

    [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.D;
    [SerializeField] private KeyCode poopKey = KeyCode.W;
    [SerializeField] private KeyCode altPoopKey = KeyCode.Space;

    [SerializeField] private float poopPressure = 0.0f;
    [SerializeField] private float poopLifetime = 10.0f;
    [SerializeField] private float minPoop = 2.0f;
    [SerializeField] private float maxPoopPressure = 10.0f;
    [SerializeField] private float poopMultiplier = 10.0f;
    [SerializeField] private const float poopDmg = 5.1f;

    private float startingHealth = 100.0f;
    private float currentHealth;

    private Rigidbody2D rb; //the player's rigibody2d

    void Start ()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        playerPhysMat = rb.sharedMaterial;
        canvasInitRot = playerCanvas.transform.rotation;
        playerNameText.text = PhotonNetwork.NickName;

        InitializeValues();
    }

    private void Update()
    {
        playerCanvas.transform.rotation = canvasInitRot;
    }

    private void InitializeValues()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            playerPhysMat.bounciness = 0.0f;
            currentHealth = startingHealth;
            transform.position = GetRandomPositionInArena();
            rb.velocity = Vector2.zero;
        }
    }

    private Vector3 GetRandomPositionInArena()
    {
        Vector3 position = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(-9.0f, 9.0f), 0);
        return position;
    }

    // Update is called once per frame
	void FixedUpdate ()
    {
        InputCheck();
	}

    private void Respawn()
    {
        //for now, this just sets the player back to 0,0,0
        //later, this will respawn the player at one of the spawn points
        InitializeValues();     
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            Respawn();
            //TODO: Lose a life
        }
    }

    private void InputCheck()
    {

        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            if (Input.GetKey(poopKey) || Input.GetKey(altPoopKey))
            {
                if (poopPressure < maxPoopPressure)
                {
                    poopPressure += Time.deltaTime * poopMultiplier;
                }
                else
                {
                    poopPressure = maxPoopPressure;
                }
            }
            else if (poopPressure > 0.0f) //if(Input.GetKeyUp(poopKey) || Input.GetKeyUp(altPoopKey))
            {
                poopPressure = poopPressure + minPoop;

                if (poopPressure >= maxPoopPressure)
                {
                    rb.AddRelativeForce(poopPressure * Vector2.up, ForceMode2D.Impulse);
                    Debug.Log("Spawning poop!");
                    GameObject tempPlaceholder = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                    GameObject tempPlaceholder1 = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                    GameObject tempPlaceholder2 = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                    GameObject tempPlaceholder3 = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                    GameObject tempPlaceholder4 = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                    Destroy(tempPlaceholder, poopLifetime);
                    Destroy(tempPlaceholder1, poopLifetime);
                    Destroy(tempPlaceholder2, poopLifetime);
                    Destroy(tempPlaceholder3, poopLifetime);
                    Destroy(tempPlaceholder4, poopLifetime);
                    poopPressure = 0.0f;
                }
                else
                {
                    rb.AddRelativeForce(poopPressure * Vector2.up, ForceMode2D.Impulse);
                    Debug.Log("Spawning poop!");
                    GameObject tempPlaceholder = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                    Destroy(tempPlaceholder, poopLifetime);
                    poopPressure = 0.0f;
                }
                      
            }

            if (Input.GetKey(rotateRightKey))
            {
                transform.Rotate(Vector3.back * rotSpeed);
            }
            else if (Input.GetKey(rotateLeftKey))
            {
                transform.Rotate(Vector3.forward * rotSpeed);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("arena"))
        {
            if (currentHealth > 0.0f)
            {
                currentHealth -= poopDmg; 
            }

            if (playerPhysMat.bounciness < 1.0f)
            {
                playerPhysMat.bounciness += 0.01f; 
            }

            Debug.Log("Health:" + currentHealth);
            Debug.Log("Bounciness:" + playerPhysMat.bounciness);
        }
    }

}
