using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Input = UnityEngine.Input;

namespace poops_Namespace
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public GameObject playerPrefab;

       
        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Player prefab is null!");
            }
            else
            {
                SpawnPlayer();
            }
        }

        /// <summary>
        /// Spawns the player in the arena. Position gets set at the PlayerManager.
        /// </summary>
        private void SpawnPlayer()
        {
            PhotonNetwork.Instantiate(this.playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        }

        public override void OnLeftRoom()
        {
            //The scene at index 0 should always be the launcher scene!
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Called by the Leave Game button's OnClick()
        /// </summary>
        public void LeaveRoom()
        {
            //calls back OnLeftRoom, thus loading the launcher scene!
            PhotonNetwork.LeaveRoom();
        }

        public void RageQuit()
        {
            Application.Quit();
            Debug.Log("RAGE QUIT!");
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Trying to load a level but we're not the master client!");
            }
            PhotonNetwork.LoadLevel(1); //Scene at index 1 is our arena! TODO: Consider refactoring this if more arenas are added!
        }

    } 
}
