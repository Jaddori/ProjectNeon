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
	public Transform profileMenu;
	public Text ipText;
	public Text nameText;

	private Transform _currentMenu;
	private string _playerName;

	private void Awake()
	{
		_currentMenu = profileMenu;
	}

	public void GotoMainMenu()
	{
		_currentMenu.gameObject.SetActive( false );
		mainMenu.gameObject.SetActive( true );

		_currentMenu = mainMenu;
	}

	public void GotoJoinMenu()
	{
		_currentMenu.gameObject.SetActive( false );

		joinMenu.gameObject.SetActive( true );

		_currentMenu = joinMenu;
	}

	public void HostGame()
	{
		netManager.Host( _playerName );
	}

	public void JoinGame()
	{
		var ip = ipText.text;
		
		netManager.Join( ip, _playerName );
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void ConfirmName()
	{
		_playerName = nameText.text;

		GotoMainMenu();
	}
}
