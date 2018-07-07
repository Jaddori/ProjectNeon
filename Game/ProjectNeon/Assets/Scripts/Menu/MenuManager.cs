using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	public Transform mainMenu;
	public Transform lobby;
	public Transform joinMenu;

	private Transform _currentMenu;

	private void Awake()
	{
		_currentMenu = mainMenu;
	}

	public void GotoMainMenu()
	{
		_currentMenu.gameObject.SetActive( false );
		mainMenu.gameObject.SetActive( true );

		_currentMenu = mainMenu;
	}

	public void GotoLobby()
	{
		_currentMenu.gameObject.SetActive( false );

		var lobbyScript = lobby.gameObject.GetComponent<Lobby>();
		lobbyScript.UpdatePlayerLabels();

		lobby.gameObject.SetActive( true );
		_currentMenu = lobby;
	}

	public void GotoJoinMenu()
	{
		_currentMenu.gameObject.SetActive( false );

		joinMenu.gameObject.SetActive( true );

		_currentMenu = joinMenu;
	}
}
