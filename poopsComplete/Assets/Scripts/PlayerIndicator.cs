using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField] private Transform player;

    /// <summary>
    /// Script is to be added on an indicator object, showing where the player is
    /// when he is not inside the camera's view!
    /// </summary>

	// Update is called once per frame
	void Update ()
    {
		transform.LookAt(player.position);
	}
}
