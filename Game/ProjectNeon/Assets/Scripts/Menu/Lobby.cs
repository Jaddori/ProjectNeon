using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Lobby : MonoBehaviour
{
	public Text[] playerLabels;

	private string[] _playerNames;
	private int _currentPlayer;

	private void Start()
	{
		_playerNames = new string[4];
		_currentPlayer = 0;
	}

	public void AddPlayer( string name )
	{
		_playerNames[_currentPlayer] = name;
		_currentPlayer++;

		UpdateLabels();
	}

	public void UpdateLabels()
	{
		for( int i = 0; i < _currentPlayer; i++ )
		{
			playerLabels[i].text = _playerNames[i];
		}
	}
}
