using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace poops_Namespace
{
    [RequireComponent(typeof(InputField))]
    public class PlayerName_InputField : MonoBehaviour
    {
        private string defaultName = string.Empty;

        private InputField inputField;

        //saving the key in a const to avoid typos!
        private const string PlayerName_PrefKey = "PlayerName";

        // Use this for initialization
        void Start()
        {
            inputField = GetComponent<InputField>();

            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(PlayerName_PrefKey))
                {
                    defaultName = PlayerPrefs.GetString(PlayerName_PrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Called from the input field's "OnValueChange" field!
        /// </summary>
        /// <param name="value"></param>
        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.Log("Name is null or empty!");
                return;
            }

            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(PlayerName_PrefKey, value);
        }
    } 
}
