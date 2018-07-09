using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
	public class NameMessage : MessageBase
	{
		public string name;
	}

	public const short SetName = 50;

	public Dictionary<int, string> playerNames = new Dictionary<int, string>();
	public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

	public void Host()
	{
		networkPort = 7777;
		StartHost();
	}

	public void Join( string ip )
	{
		networkAddress = ip;
		networkPort = 7777;
		StartClient();
	}

	public override void OnClientConnect( NetworkConnection conn )
	{
		base.OnClientConnect( conn );

		conn.Send( SetName, new NameMessage() { name = "Bojangles" } );
	}

	public override void OnServerConnect( NetworkConnection conn )
	{
		base.OnServerConnect( conn );

		conn.RegisterHandler( SetName, OnSetNameMessage );
	}

	public void OnSetNameMessage( NetworkMessage message )
	{
		var name = message.reader.ReadString();
		var connId = message.conn.connectionId;

		Debug.LogError( "Setting name " + name + " to connId " + connId.ToString() );

		if( !playerNames.ContainsKey( connId ) )
		{
			playerNames.Add( connId, name );
		}
	}

	public override void OnServerAddPlayer( NetworkConnection conn, short playerControllerId )
	{
		var player = Instantiate( playerPrefab, transform.position, Quaternion.identity );
		NetworkServer.AddPlayerForConnection( conn, player, playerControllerId );

		var connId = conn.connectionId;
		if( playerNames.ContainsKey( connId ) )
		{
			var playerScript = player.GetComponent<Player>();
			var name = playerNames[connId];
			playerScript.playerName = name;

			players.Add( connId, player );
		}
	}
}
