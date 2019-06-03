using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Experimental.UIElements;

namespace poops_Namespace
{
    public class Launcher : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        [SerializeField] private GameObject progressLabel;
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject howToPlayPanel;
        [SerializeField] private GameObject joinGamePanel;
        [SerializeField] private GameObject createGamePanel;

        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private GameObject roomScrollView;

        [SerializeField] private GameObject gameSettingsGO;

        [SerializeField] private byte maxPlayers = 4; //this allows a total of 5 rooms with the free version (5 maxed rooms)

        private string gameVersion = "1"; //only updated when a MAJOR update is up!

        private bool isConnecting;

        private GameSettings gameSettings;

        private string roomName = "";

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            DontDestroyOnLoad(gameSettingsGO);
            gameSettings = gameSettingsGO.GetComponent<GameSettings>();

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings(); 
            }
        }

        // Use this for initialization
        void Start()
        {
            controlPanel.SetActive(true);
            howToPlayPanel.SetActive(false);
            progressLabel.SetActive(false);
            joinGamePanel.SetActive(false);
            createGamePanel.SetActive(false);
        }

        //called when the roomName input field is edited!
        public void SetRoomName(string value)
        {
            roomName = value;
        }

        public void Connect()
        {
            isConnecting = true;

            progressLabel.SetActive(true);
            createGamePanel.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {
                //PhotonNetwork.JoinRandomRoom();
                SetupRoomWithSettings();
            }
        }

        //called from the 'Create Room' button within the 'Create Room' panel, through the "Connect()" method,
        //after setting up name, max players and best of rounds!
        public void SetupRoomWithSettings()
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = gameSettings.GetNumOfPlayers() });
        }

        public void CreateRoomButton()
        {
            controlPanel.SetActive(false);
            createGamePanel.SetActive(true);
        }

        public void JoinRoomButton()
        {
            controlPanel.SetActive(false);
            joinGamePanel.SetActive(true);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList) //TODO: THIS DOES NOT GET CALLED, FIX IT TO SEE ROOM LIST!
        {
            Debug.Log("Called OnRoomListUpdate!");
            base.OnRoomListUpdate(roomList);

            foreach (RoomInfo rInfo in roomList)
            {
                GameObject rPrefab = Instantiate(roomPrefab, transform.position, transform.rotation);

                int currPlayers = rInfo.PlayerCount;
                int maxPlayers = rInfo.MaxPlayers;
                string roomName = rInfo.Name;

                rPrefab.transform.Find("Current Players").GetComponent<Text>().text = currPlayers.ToString();
                rPrefab.transform.Find("Max Players").GetComponent<Text>().text = maxPlayers.ToString();
                rPrefab.transform.Find("Room Name").GetComponent<Text>().text = roomName;

                rPrefab.transform.SetParent(roomScrollView.transform);
            }
        }

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                PhotonNetwork.JoinRoom(this.roomName);
            }
        }

        //This should no longer be used since we're manually selecting and joining a room, or creating one!
        //public override void OnJoinRandomFailed(short returnCode, string message)
        //{
        //    //if we failed to join a random room, create a new one, and set maxplayers to the chosen value
        //    PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = maxPlayers});
        //}

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

        public void HowToPlayButton()
        {
            howToPlayPanel.SetActive(true);
            controlPanel.SetActive(false);
        }

        public void BackToMainMenuButton()
        {
            howToPlayPanel.SetActive(false);
            joinGamePanel.SetActive(false);
            createGamePanel.SetActive(false);
            controlPanel.SetActive(true);
        }
        
        public void ExitButton()
        {
            Application.Quit();
            Debug.Log("Game Closed");
        }
    } 
}
