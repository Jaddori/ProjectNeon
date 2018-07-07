using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
	[SerializeField]
	public GameObject enemy;

	[SerializeField]
	public int enemyCount;

	public bool _hasSpawned;
	
	private void Start()
	{
		_hasSpawned = false;
	}

	public void Spawn()
	{
		CmdSpawnOnServer();
	}

	[Command]
	private void CmdSpawnOnServer()
	{
		if( !_hasSpawned )
		{
			_hasSpawned = true;

			for( int i = 0; i < enemyCount; i++ )
			{
				var enemyInstance = Instantiate( enemy, transform.position, Quaternion.identity );
				NetworkServer.Spawn( enemyInstance );
			}
		}
	}
}
