using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
	public Text[] playerLabels;
	private string[] playerNames;

	public void UpdatePlayerLabels()
	{
		if( playerNames == null )
		{
			playerNames = new string[] { "Bojangles", string.Empty, string.Empty, string.Empty };
		}

		for( int i = 0; i < playerLabels.Length; i++ )
		{
			playerLabels[i].text = playerNames[i];
		}
	}
}
