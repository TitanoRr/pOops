using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace poops_Namespace
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject progressLabel;
        [SerializeField] private GameObject controlPanel;

        [SerializeField] private byte maxPlayers = 6;

        private string gameVersion = "1"; //only updated when a MAJOR update is up!

        private bool isConnecting;

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Use this for initialization
        void Start()
        {
            controlPanel.SetActive(true);
            progressLabel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Connect()
        {
            isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            //if we failed to join a random room, create a new one, and set maxplayers to the chosen value
            PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = maxPlayers});
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected with cause: {0}" + cause);
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined room!");
            PhotonNetwork.LoadLevel(1); //Scene at index 1 is the arena! This may be refactored if more arenas are added!
        }
    } 
}
