using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField]
	public float height = 2.0f;

	[SerializeField]
	public float distance = 5.0f;

	private GameObject _player;

	private void Start()
	{
		_player = null;

		var players = GameObject.FindGameObjectsWithTag( "Player" );
		for( int i = 0; i < players.Length && _player == null; i++ )
		{
			var script = players[i].GetComponent<Player>();
			if( script != null && script.isLocalPlayer )
				_player = players[i];
		}
	}

	private void LateUpdate()
	{
		if( _player != null )
		{
			//Vector3 playerPosition = _player.transform.position;
			//Vector3 playerForward = _player.transform.forward;
			//
			//Vector3 newPosition = playerPosition - playerForward * distance;
			//newPosition.y = height;
			//
			//transform.position = newPosition;
			//transform.rotation = Quaternion.LookRotation( playerPosition - newPosition );

			var playerPosition = _player.transform.position;
			var relativePosition = new Vector3( 0, -height, distance );

			transform.position = playerPosition - relativePosition;
			transform.rotation = Quaternion.LookRotation( playerPosition - transform.position );
		}
	}
}
