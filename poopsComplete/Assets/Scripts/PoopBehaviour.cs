using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PoopBehaviour : MonoBehaviour
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

    //TODO: make this work!
    //void Update()
    //{
    //    if (timer < poopLifetime)
    //    {
    //        timer += Time.deltaTime;
    //    }
    //    else if (timer >= poopLifetime)
    //    {
    //        PhotonNetwork.Destroy(this.gameObject);
    //    }
    //}

    private void Fly()
    {
        rb.AddRelativeForce(Vector3.up * poopFlyingSpeed);
    }
}
