using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadyTrigger : MonoBehaviour
{
	public int Color;

	private NetManager _netManager;
	private GameObject _player;

	public void Awake()
	{
		var netManagerObject = GameObject.FindGameObjectWithTag( "NetManager" );
		_netManager = netManagerObject.GetComponent<NetManager>();
	}

	public void Update()
	{
		if( _player != null )
		{
			if( Input.GetKey( KeyCode.Space ) )
			{
				_netManager.ServerChangeScene( "Sandbox" );
			}
		}
	}

	public void OnTriggerEnter( Collider collider )
	{
		if( _player == null )
		{
			_player = collider.gameObject;
		}
	}

	public void OnTriggerExit( Collider collider )
	{
		if( collider.gameObject == _player )
			_player = null;
	}
}
