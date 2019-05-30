using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

	// Use this for initialization
	void Start ()
    {
        pausePanel.SetActive(false);	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                pausePanel.SetActive(false); 
            } 
            else
            {
                pausePanel.SetActive(true); 
            }
        }
	}
}
