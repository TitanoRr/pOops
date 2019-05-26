using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MOVING THE WHOLE FUNCTIONALITY INSIDE THE PLAYER, SHOULD WORK BETTER!
/// </summary>
public class ChargeBarBehaviour : MonoBehaviour
{

    //[SerializeField] private GameObject player;

    //private PlayerManager playerManager;

    //private Slider chargeSlider;

    //private float currentChargeStatus;

    // Use this for initialization
	void Start ()
    {
        //chargeSlider = GetComponent<Slider>();
        //chargeSlider.minValue = 0.0f;
        //chargeSlider.maxValue = 10.0f;
        //chargeSlider.value = 0.0f;

        //playerManager = player.GetComponent<PlayerManager>();
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		//SetChargeUI();
	}

    //private void SetChargeUI()
    //{

    //    if (playerManager.GetPoopCharge() > playerManager.GetMaxPoopCharge())
    //    {
    //        Debug.Log("Charge is max!");
    //        chargeSlider.value = playerManager.GetMaxPoopCharge();
    //        Debug.Log("Value set to max poop charge value!");
    //    }
    //    else
    //    {
    //        Debug.Log("Charge is not max!");
    //        chargeSlider.value = playerManager.GetPoopCharge();
    //        Debug.Log("Value set to: " + chargeSlider.value);
    //    }
    //}
}
