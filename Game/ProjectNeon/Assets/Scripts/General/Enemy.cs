using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	private const float DISTANCE_MAX = 10.0f;
	private const float DISTANCE_MIN = 4.5f;
	private const float SPEED = 0.1f;

	private GameObject _player;
	private NavMeshAgent _navMeshAgent;

	private void Awake()
	{
		_player = GameObject.FindGameObjectWithTag( "Player" );
		_navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		var direction = ( _player.transform.position - transform.position );
		var distance = direction.magnitude;

		RaycastHit hitInfo;
		var rayHit = Physics.Raycast( transform.position, direction.normalized, out hitInfo, distance );
		var playerVisible = ( !rayHit || hitInfo.collider.gameObject == _player );

		if( distance > DISTANCE_MAX )
			_navMeshAgent.destination = _player.transform.position;
		else if( distance < DISTANCE_MIN )
		{
			if( playerVisible )
			{
				var backupDistance = DISTANCE_MIN - distance;
				_navMeshAgent.destination = transform.position - direction * backupDistance;
			}
			else
				_navMeshAgent.destination = _player.transform.position;
		}
		else // DISTANCE_MIN < distance < DISTANCE_MAX
		{
			if( playerVisible )
			{
				_navMeshAgent.isStopped = true;
				_navMeshAgent.ResetPath();
			}
			else
				_navMeshAgent.destination = _player.transform.position;
		}
	}
}
