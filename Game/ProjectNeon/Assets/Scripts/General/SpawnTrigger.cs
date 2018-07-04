using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
	[SerializeField]
	public GameObject spawner;

	private EnemySpawner _spawnerScript;

	private void Awake()
	{
		_spawnerScript = spawner.GetComponent<EnemySpawner>();
	}

	public void OnTriggerEnter( Collider other )
	{
		Debug.Log( "Entered trigger." );

		_spawnerScript.Spawn();
		gameObject.SetActive( false );
	}
}
