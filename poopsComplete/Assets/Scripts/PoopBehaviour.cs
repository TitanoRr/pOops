using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PoopBehaviour : MonoBehaviourPunCallbacks
{
    [SerializeField] private int poopFlyingSpeed;
    [SerializeField] private float poopLifetime = 10.0f;

    private float timer = 0.0f;

    private Rigidbody2D rb; //the poop's rigidbody2d

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        Fly();
    }

    void Update()
    {
        if (timer < poopLifetime) //if we haven't lived long enough
        {
            timer += Time.deltaTime; //add the time since last frame in the timer var
        }
        else if (timer >= poopLifetime) //else it's time to go.. :(
        {
            //PhotonNetwork.Destroy(this.gameObject); //destroying through network fixes the poop-shotgun issue
            Destroy(this.gameObject);
        }
    }

    private void Fly()
    {
        rb.AddRelativeForce(Vector3.up * poopFlyingSpeed);
    }
}
