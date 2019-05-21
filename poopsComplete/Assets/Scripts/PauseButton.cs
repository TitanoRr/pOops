using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject exitButton;

	// Use this for initialization
	void Start ()
    {
	    exitButton.SetActive(false);	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitButton.activeSelf)
            {
                exitButton.SetActive(false); 
            } 
            else
            {
                exitButton.SetActive(true);
            }
        }
	}
}
