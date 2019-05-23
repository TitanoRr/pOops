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
                Vector3 position = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(-9.0f, 9.0f), 0);
                PhotonNetwork.Instantiate(this.playerPrefab.name,position, Quaternion.identity, 0);
            }
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

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Trying to load a level but we're not the master client!");
            }
            PhotonNetwork.LoadLevel(1); //Scene at index 1 is our arena! TODO: Consider refactoring this if more arenas are added!
        }

        ///Since we're using only 1 arena, not based on number of players, we don't have to load a new arena every time a player enters the room!
        //public override void OnPlayerEnteredRoom(Player newPlayer)
        //{
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        LoadArena();
        //    }
        //}
    } 
}
