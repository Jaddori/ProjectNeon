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
}
