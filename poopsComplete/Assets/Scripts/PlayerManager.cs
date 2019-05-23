using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    [SerializeField] private GameObject poop; //the poop prefab to be pooped!

    [SerializeField] private Transform poopSpawner; //the poop spawner transform the player has

    [SerializeField] private int poopSpeed = 10; //the player's move speed
    [SerializeField] private int rotSpeed = 10; //the player's rotation speed

    [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.D;
    [SerializeField] private KeyCode poopKey = KeyCode.W;
    [SerializeField] private KeyCode altPoopKey = KeyCode.Space;

    [SerializeField] private float poopPressure = 0.0f;
    [SerializeField] private float poopLifetime = 10.0f;
    [SerializeField] private float minPoop = 2.0f;
    [SerializeField] private float maxPoopPressure = 10.0f;
    [SerializeField] private float poopMultiplier = 10.0f;

    private Rigidbody2D rb; //the player's rigibody2d

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();

        InitializeValues();
    }

    private void InitializeValues()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {      
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

                rb.AddRelativeForce(poopPressure * Vector2.up, ForceMode2D.Impulse);
                Debug.Log("Spawning poop!");
                GameObject tempPlaceholder = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                Destroy(tempPlaceholder, poopLifetime);
                poopPressure = 0.0f;           
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
}
