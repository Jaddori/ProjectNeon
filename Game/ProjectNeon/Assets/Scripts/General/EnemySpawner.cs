using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField]
	public GameObject enemy;

	[SerializeField]
	public int enemyCount;
	
	private void Start()
	{
	}

	public void Spawn()
	{
		Debug.Log( "Should spawn." );

		for( int i = 0; i < enemyCount; i++ )
		{
			Instantiate( enemy, transform.position, Quaternion.identity );
		}
	}
}
