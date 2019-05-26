﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject poop; //the poop prefab to be pooped!
    [SerializeField] private GameObject playerCanvas;

    [SerializeField] private Text playerNameText;

    [SerializeField] private Transform poopSpawner; //the poop spawner transform the player has

    [SerializeField] private int shotgunShells = 5; //the amount of poop in the full charge shot
    [SerializeField] private int rotSpeed = 10; //the player's rotation speed

    [SerializeField] private Quaternion canvasInitRot; //initial canvas rotation

    [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.D;
    [SerializeField] private KeyCode poopKey = KeyCode.W;
    [SerializeField] private KeyCode altPoopKey = KeyCode.Space;

    [SerializeField] private float poopCharge = 0.0f;
    [SerializeField] private float poopLifetime = 10.0f;
    [SerializeField] private float minPoop = 2.0f;
    [SerializeField] private float maxPoopCharge = 10.0f;
    [SerializeField] private float poopMultiplier = 10.0f;
    [SerializeField] private float startingMass = 2.1f;
    [SerializeField] private float massPerHit = 0.1f;

    private const float poopDmg = 10.0f; //10 hits --> minimum mass --> red name!
    private const float colorDmg = 51.0f; //the value the name gets/loses when hit to turn red!

    private Slider chargeSlider;

    private GameObject chargeSliderFill;

    private float startingHealth = 100.0f;
    private float currentHealth;

    private Rigidbody2D rb; //the player's rigibody2d

    void Start ()
    {

        if (photonView.IsMine && PhotonNetwork.IsConnected) //if the prefab is the local player
        {
            rb = GetComponentInParent<Rigidbody2D>();

            canvasInitRot = playerCanvas.transform.rotation;

            chargeSliderFill = GameObject.FindGameObjectWithTag("chargeSliderFill");
            chargeSliderFill.SetActive(false);

            chargeSlider = GameObject.FindGameObjectWithTag("chargeSlider").GetComponent<Slider>();
            chargeSlider.minValue = 0.0f;
            chargeSlider.maxValue = 10.0f;

            playerNameText.text = PhotonNetwork.NickName; //set nickname 
        }

        InitializeValues();
    }

    void Update()
    {
        playerCanvas.transform.rotation = canvasInitRot;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputCheck();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerNameText.text);
        }
        else
        {
            playerNameText.text = (string)stream.ReceiveNext();
        }
    }

    //Called in "OnValueChange" field of the slider, purely for aesthetic reasons!
    public void DisableFillIfZero()
    {
        chargeSliderFill.SetActive(chargeSlider.value > 0.0f); //if we're charging, enable fill, otherwise disable
    }

    private void SetChargeUI()
    {
        if (poopCharge > maxPoopCharge)
        {
            chargeSlider.value = maxPoopCharge;
        }
        else
        {
            chargeSlider.value = poopCharge;
        }
    }

    private void InitializeValues()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected) //if the player prefab is the local player...
        {
            currentHealth = startingHealth; //reset health
            transform.position = GetRandomPositionInArena(); //set random position
            rb.velocity = Vector2.zero; //reset velocity
            playerNameText.color = new Color(0.0f, 255.0f, 0.0f); //pure green!
            rb.mass = 2.1f;

            chargeSlider.value = 0.0f; //who spawns mid-charge!?
        }
    }

    private Vector3 GetRandomPositionInArena()
    {
        Vector3 position = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(-9.0f, 9.0f), 0);
        return position;
    }

    private void Respawn()
    {
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
                if (poopCharge < maxPoopCharge)
                {
                    poopCharge += Time.deltaTime * poopMultiplier;
                }
                else
                {
                    poopCharge = maxPoopCharge;
                }

                SetChargeUI();
            }
            else if (poopCharge > 0.0f)
            {
                poopCharge += minPoop;

                if (poopCharge >= maxPoopCharge)
                {
                    SpawnPoopThroughNetwork(shotgunShells); //shotgun shot (full charge)
                }
                else
                {
                    SpawnPoopThroughNetwork(1); //single shot (not full charge)
                }

                SetChargeUI();
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

    private void SpawnPoopThroughNetwork(int amountToSpawn)
    {
        rb.AddRelativeForce(poopCharge * Vector2.up, ForceMode2D.Impulse);

        for (int i = 0; i < amountToSpawn; i++)
        {
            PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
        }

        poopCharge = 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("arena")) //if we're hitting anything BUT the arena...
        {
            if (currentHealth > 0.0f)
            {
                currentHealth -= poopDmg;
                rb.mass -= massPerHit;

                if (playerNameText.color.r < 255) //first fill up red
                {
                    float red = colorDmg + playerNameText.color.r;
                    playerNameText.color = new Color(red, playerNameText.color.g, playerNameText.color.b,
                        playerNameText.color.a);
                    Debug.Log("Red is " + playerNameText.color.r);
                }
                else if (playerNameText.color.g > 0) //then empty out green
                {
                    float green = playerNameText.color.g - colorDmg;
                    playerNameText.color = new Color(playerNameText.color.r, green, playerNameText.color.b,
                        playerNameText.color.a);
                    Debug.Log("Green is " + playerNameText.color.g);
                }
            }

        }
    }
}
