using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    [SerializeField] private GameObject poop; //the poop prefab to be pooped!
    [SerializeField] private Transform poopSpawner; //the poop spawner transform the player has

    [SerializeField] private float poopPressure;
    private float minPoop;
    private float maxPoopPressure;

    [SerializeField] private float poopLifetime = 10.0f;

    [SerializeField] private int poopSpeed = 10; //the player's move speed
    [SerializeField] private int rotSpeed = 10; //the player's rotation speed

    [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.D;
    [SerializeField] private KeyCode poopKey = KeyCode.W;
    [SerializeField] private KeyCode altPoopKey = KeyCode.Space;
    

    private Rigidbody2D rb; //the player's rigibody2d

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        poopPressure = 0f;
        minPoop = 2f;
        maxPoopPressure = 10f;
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
        rb.velocity = Vector2.zero;
        transform.position = Vector3.zero;
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
                    poopPressure += Time.deltaTime * 10f;
                }
                else
                {
                    poopPressure = maxPoopPressure;
                }
            }
            else //if(Input.GetKeyUp(poopKey) || Input.GetKeyUp(altPoopKey))
            {
                if(poopPressure > 0f)
                {
                    poopPressure = poopPressure + minPoop;
                    
                    rb.AddRelativeForce(poopPressure * Vector2.up, ForceMode2D.Impulse);
                    GameObject tempPlaceholder = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
                    Destroy(tempPlaceholder, poopLifetime);
                    poopPressure = 0f;
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

    /*private void Poop()
    {
        rb.AddRelativeForce(poopSpeed * Vector2.up, ForceMode2D.Impulse);
        GameObject tempPlaceholder = PhotonNetwork.Instantiate("poop", poopSpawner.position, poopSpawner.rotation);
        Destroy(tempPlaceholder, poopLifetime); //destroys the poop after 10 seconds
        
    }*/
}
