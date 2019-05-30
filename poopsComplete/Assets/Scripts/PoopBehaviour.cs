using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PoopBehaviour : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte _poopFlyingSpeed;

    private readonly byte _poopLifetime = 10;
    private readonly byte _sendRate = 50; //if this is changed, make sure to check if the PlayerManager's respective var needs to change too!

    private float _timer = 0.0f;

    private Rigidbody2D _rb; //the poop's rigidbody2d

	void Start ()
    {
        //These 2 lines should reduce lag and improve experience overall! 
        PhotonNetwork.SendRate = _sendRate;
        PhotonNetwork.SerializationRate = _sendRate;


        _rb = GetComponent<Rigidbody2D>();
        Fly();
    }

    void Update()
    {
        if (_timer < _poopLifetime) //if we haven't lived long enough
        {
            _timer += Time.deltaTime; //add the time since last frame in the _timer var
        }
        else if (_timer >= _poopLifetime) //else it's time to go.. :(
        {
            if (photonView.IsMine && PhotonNetwork.IsConnected)
            {
                //destroying through network fixes the poop-shotgun issue
                PhotonNetwork.Destroy(this.gameObject); 
            }
            else
            {
                Destroy(this.gameObject); 
            }

        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void Fly()
    {
        _rb.AddRelativeForce(Vector3.up * _poopFlyingSpeed);
    }
}
