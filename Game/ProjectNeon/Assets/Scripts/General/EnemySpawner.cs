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

			var radialChunk = 360.0f / enemyCount;
			var linearChunk = enemyCount*0.5f;
			for( int i = 0; i < enemyCount; i++ )
			{
				var curChunk = radialChunk * i * Mathf.Deg2Rad;
				var position = new Vector3( Mathf.Cos( curChunk ), 0.0f, Mathf.Sin( curChunk ) ) * linearChunk;
				position.y = 1.0f;

				var enemyInstance = Instantiate( enemy, transform.position + position, Quaternion.identity );
				//var enemyInstance = Instantiate( enemy, transform.position, Quaternion.identity );
				NetworkServer.Spawn( enemyInstance.gameObject );
			}
		}
	}
}
