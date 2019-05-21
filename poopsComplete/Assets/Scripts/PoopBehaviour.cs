using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopBehaviour : MonoBehaviour
{
    [SerializeField] private int poopFlyingSpeed = 1;

    private Rigidbody2D rb; //the poop's rigidbody2d

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        Fly();
    }


    private void Fly()
    {
        rb.AddRelativeForce(Vector3.up * poopFlyingSpeed);
    }
}
