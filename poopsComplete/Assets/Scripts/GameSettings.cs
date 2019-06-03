using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private Slider _bestOfSlider;
    [SerializeField] private Text _bestOfText;

    [SerializeField] private Slider _numOfPlayersSlider;
    [SerializeField] private Text _numOfPlayersText;

    private const string BEST_OF_ROUNDS_PREFIX_TEXT = "Rounds Best Of: ";
    private const string NUM_OF_PLAYERS_PREFIX_TEXT = "Number of Players: ";

    private static int _bestOfRounds;
    private static byte _numOfPlayers = 2;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetRoundsToWin()
    {
        //For 2 players---v
        //numOfPlayers * 1 + 1 = (best of) 3 // 2 to win
        //numOfPlayers * 2 + 1= (best of) 5 // 3 to win
        //numOfPlayers * 3 + 1 = (best of) 7 // 4 to win

        _numOfPlayers = (byte)_numOfPlayersSlider.value;
        _bestOfRounds = _numOfPlayers * (int)_bestOfSlider.value + 1;

        _numOfPlayersText.text = NUM_OF_PLAYERS_PREFIX_TEXT + _numOfPlayers.ToString();
        _bestOfText.text = BEST_OF_ROUNDS_PREFIX_TEXT + _bestOfRounds.ToString();
    }

    public byte GetNumOfPlayers()
    {
        return _numOfPlayers;
    }

    public int GetBestOfRounds()
    {
        return _bestOfRounds;
    }
}
