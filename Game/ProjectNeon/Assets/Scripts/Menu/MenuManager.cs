using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public NetManager netManager;

	public Transform mainMenu;
	public Transform lobby;
	public Transform joinMenu;
	public Text ipText;

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

		lobby.gameObject.SetActive( true );
		_currentMenu = lobby;
	}

	public void GotoJoinMenu()
	{
		_currentMenu.gameObject.SetActive( false );

		joinMenu.gameObject.SetActive( true );

		_currentMenu = joinMenu;
	}

	public void HostGame()
	{
		netManager.Host();
	}

	public void JoinGame()
	{
		var ip = ipText.text;
		
		netManager.Join( ip );
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
