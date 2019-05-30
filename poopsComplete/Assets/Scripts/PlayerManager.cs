using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{ 

    //***THIS IS USED BUT DUE TO THE PHOTONNETWORK.INSTANTIATE NEEDING IT IN A STRING, IT APPEARS AS UNUSED!***\\ 
    [SerializeField] private GameObject _poop; //the _poop prefab to be pooped!
    [SerializeField] private GameObject _playerCanvas;
    [SerializeField] private GameObject _helmetGlass;

    [SerializeField] private Text _playerNameText;

    [SerializeField] private Transform _poopSpawner; //the _poop spawner transform the player has

    [SerializeField] private byte _sendRate = 50; //the _sendRate and syncSendRate of photon! if this is changed, make sure to check if the poopBehaviour's respective var needs to change too!
    [SerializeField] private byte _shotgunShells = 5; //the amount of _poop in the full charge shot
    [SerializeField] private byte _rotSpeed = 10; //the player's rotation speed

    [SerializeField] private Quaternion _canvasInitRot; //initial canvas rotation

    [SerializeField] private KeyCode _rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode _rotateRightKey = KeyCode.D;
    [SerializeField] private KeyCode _poopKey = KeyCode.W;
    [SerializeField] private KeyCode _altPoopKey = KeyCode.Space;

    [SerializeField] [SuppressMessage("ReSharper", "InconsistentNaming")]
    private const float STARTING_MASS = 2.1f;
    [SerializeField] [SuppressMessage("ReSharper", "InconsistentNaming")]
    private const float MIN_POOP = 2.0f;
    [SerializeField] [SuppressMessage("ReSharper", "InconsistentNaming")]
    private const float MAX_POOP_CHARGE = 10.0f;
    [SerializeField] [SuppressMessage("ReSharper", "InconsistentNaming")]
    private const float POOP_MULTIPLIER = 10.0f;
    [SerializeField] [SuppressMessage("ReSharper", "InconsistentNaming")]
    private const float MASS_PER_HIT = 0.1f;

    [SerializeField] private Color _playerColor;

    [SerializeField] private readonly AudioClip[] _poopFx = new AudioClip[3]; //3 effects for now!

    [SerializeField] private Slider _chargeSlider;

    private bool _isDead = false;

    private byte _myIndex;
    private byte _playerLives = 3; //this defaults to 0 but will change based on room's settings!

    private const byte POOP_DMG = 10; //10 hits --> minimum mass --> red name!
    private const byte COLOR_DMG = 51; //the value the name gets/loses when hit to turn red!

    private float _poopCharge = 0.0f;
    private float _startingHealth = 100.0f;
    private float _currentHealth;

    private AudioSource _playerAudioSource;

    private GameObject _chargeSliderFill;

    private SpriteRenderer _helmetGlassRenderer;

    private Rigidbody2D _rb; //the player's rigibody2d

    void Start ()
    {
        //These 2 lines should reduce lag and improve experience overall! 
        PhotonNetwork.SendRate = _sendRate;
        PhotonNetwork.SerializationRate = _sendRate;

        _chargeSliderFill = GameObject.FindGameObjectWithTag("chargeSliderFill");
        Debug.Log(_chargeSliderFill);
        _chargeSliderFill.SetActive(false);

        _chargeSlider.minValue = 0.0f;
        _chargeSlider.maxValue = 10.0f;

        _helmetGlassRenderer = _helmetGlass.GetComponent<SpriteRenderer>();

        if (photonView.IsMine && PhotonNetwork.IsConnected) //if the prefab is the local player
        {
            SetIndexAndAdjustHelmColor(PhotonNetwork.CurrentRoom.PlayerCount - 1);

            _rb = GetComponentInParent<Rigidbody2D>();

            _canvasInitRot = _playerCanvas.transform.rotation;

            _playerAudioSource = GetComponent<AudioSource>();

            _playerNameText.text = PhotonNetwork.NickName; //set nickname 
        }

        InitializeValues();
    }

    void Update()
    {
        _playerCanvas.transform.rotation = _canvasInitRot;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputCheck();
    }

    void SetIndexAndAdjustHelmColor(int index)
    {
        _myIndex = (byte)index;

        if (_myIndex == 0)
        {
            _playerColor = Color.blue;
        }
        else if (_myIndex == 1)
        {
            _playerColor = Color.yellow;
        }
        else if (_myIndex == 2)
        {
            _playerColor = Color.magenta;
        }
        else if (_myIndex == 3)
        {
            _playerColor = Color.red;
        }

        _helmetGlassRenderer.color = _playerColor;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_playerNameText.text);
            stream.SendNext(_myIndex);
        }
        else
        {
            _playerNameText.text = (string)stream.ReceiveNext();

            _myIndex = (byte) stream.ReceiveNext();
            SetIndexAndAdjustHelmColor(_myIndex);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _myIndex = (byte)(PhotonNetwork.CurrentRoom.PlayerCount - 1);
        SetIndexAndAdjustHelmColor(_myIndex);
    }

    //Called in "OnValueChange" field of the slider, purely for aesthetic reasons!
    public void DisableFillIfZero()
    {
        _chargeSliderFill.SetActive(_chargeSlider.value > 0.0f); //if we're charging, enable fill, otherwise disable
    }

    public bool IsDead() { return _isDead; }

    private void SetChargeUI()
    {
        if (_poopCharge > MAX_POOP_CHARGE)
        {
            _chargeSlider.value = MAX_POOP_CHARGE;
        }
        else
        {
            _chargeSlider.value = _poopCharge;
        }
    }

    private void InitializeValues()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected) //if the player prefab is the local player...
        {
            _currentHealth = _startingHealth; //reset health
            transform.position = GetRandomPositionInArena(); //set random position
            _rb.velocity = Vector2.zero; //reset velocity
            _playerNameText.color = new Color(0.0f, 255.0f, 0.0f); //pure green!
            _rb.mass = STARTING_MASS;
            _poopCharge = 0.0f;
            _chargeSlider.value = 0.0f; //who spawns mid-charge!?
        }
    }

    private Vector3 GetRandomPositionInArena()
    {
        Vector3 position = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(-9.0f, 9.0f), 0);
        return position;
    }

    private void Respawn()
    {
        if (_playerLives > 0)
        {
            _playerLives--;

            InitializeValues(); 
        }
        else
        {
            _isDead = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            Respawn(); //tries to respawn if lives>0 or dies!
        }
    }

    private void InputCheck()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            if (!_isDead)
            {
                if (Input.GetKey(_poopKey) || Input.GetKey(_altPoopKey))
                {
                    if (_poopCharge < MAX_POOP_CHARGE)
                    {
                        _poopCharge += Time.deltaTime * POOP_MULTIPLIER;
                    }
                    else
                    {
                        _poopCharge = MAX_POOP_CHARGE;
                    }

                    SetChargeUI();
                }
                else if (_poopCharge > 0.0f)
                {
                    _poopCharge += MIN_POOP;

                    if (_poopCharge >= MAX_POOP_CHARGE)
                    {
                        SpawnPoopThroughNetwork(_shotgunShells); //shotgun shot (full charge)
                    }
                    else
                    {
                        SpawnPoopThroughNetwork(1); //single shot (not full charge)
                    }

                    SetChargeUI();
                }

                if (Input.GetKey(_rotateRightKey))
                {
                    transform.Rotate(Vector3.back * _rotSpeed);
                }
                else if (Input.GetKey(_rotateLeftKey))
                {
                    transform.Rotate(Vector3.forward * _rotSpeed);
                } 
            }
        }
    }

    private void SpawnPoopThroughNetwork(int amountToSpawn)
    {
        if (amountToSpawn > 1) //this means we're doing full charge!
        {
            _playerAudioSource.clip = _poopFx[2]; //plays the _poop charge effect!
        }
        else
        {
            _playerAudioSource.clip = _poopFx[Random.Range(0, 2)]; //plays one of the 2 _poop effects, randomly! 
        }
        _playerAudioSource.Play();

        _rb.AddRelativeForce(_poopCharge * Vector2.up, ForceMode2D.Impulse);

        for (int i = 0; i < amountToSpawn; i++)
        {
            PhotonNetwork.Instantiate("Poop", _poopSpawner.position, _poopSpawner.rotation);
        }

        _poopCharge = 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("arena")) //if we're hitting anything BUT the arena...
        {
            if (_currentHealth > 0.0f)
            {
                _currentHealth -= POOP_DMG;
                _rb.mass -= MASS_PER_HIT;

                if (_playerNameText.color.r < 255) //first fill up red
                {
                    float red = COLOR_DMG + _playerNameText.color.r;
                    _playerNameText.color = new Color(red, _playerNameText.color.g, _playerNameText.color.b,
                        _playerNameText.color.a);
                }
                else if (_playerNameText.color.g > 0) //then empty out green
                {
                    float green = _playerNameText.color.g - COLOR_DMG;
                    _playerNameText.color = new Color(_playerNameText.color.r, green, _playerNameText.color.b,
                        _playerNameText.color.a);
                }
            }

        }
    }
}
