using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour
{
	private const float DISTANCE_MAX = 10.0f;
	private const float DISTANCE_MIN = 4.5f;
	private const float SPEED = 0.1f;
	private const float TARGET_CONSIDERATION_DELAY = 5.0f;
	private const float POSITION_THRESHOLD = 2.0f;

	private GameObject[] _players;
	private GameObject _target;
	private NavMeshAgent _navMeshAgent;
	private float _targetConsiderationDelay;

	[SyncVar]
	private EnemyState _state;

	private void Start()
	{
		if( isServer )
		{
			_players = GameObject.FindGameObjectsWithTag( "Player" );

			_navMeshAgent = GetComponent<NavMeshAgent>();
			_targetConsiderationDelay = 0.0f;
		}
	}

	private void Update()
	{
		if( isServer )
		{
			_targetConsiderationDelay -= Time.deltaTime;

			// find the closest target
			if( _targetConsiderationDelay <= 0 )
			{
				_target = FindClosestPlayer();
				_targetConsiderationDelay = TARGET_CONSIDERATION_DELAY;
			}

			// move toward target
			var direction = ( _target.transform.position - transform.position );
			var distance = direction.magnitude;

			RaycastHit hitInfo;
			var rayHit = Physics.Raycast( transform.position, direction.normalized, out hitInfo, distance );
			var playerVisible = ( !rayHit || hitInfo.collider.gameObject == _target );

			if( distance > DISTANCE_MAX )
				_navMeshAgent.destination = _target.transform.position;
			else if( distance < DISTANCE_MIN )
			{
				if( playerVisible )
				{
					var backupDistance = DISTANCE_MIN - distance;
					_navMeshAgent.destination = transform.position - direction * backupDistance;
				}
				else
					_navMeshAgent.destination = _target.transform.position;
			}
			else // DISTANCE_MIN < distance < DISTANCE_MAX
			{
				if( playerVisible )
				{
					_navMeshAgent.isStopped = true;
					_navMeshAgent.ResetPath();
				}
				else
					_navMeshAgent.destination = _target.transform.position;
			}
		}
	}

	private void FixedUpdate()
	{
		SyncState();
	}

	private GameObject FindClosestPlayer()
	{
		GameObject closestPlayer = null;
		var closestDistance = float.MaxValue;
		foreach( var player in _players )
		{
			var distance = Vector3.Distance( player.transform.position, transform.position );
			if( distance < closestDistance )
			{
				closestPlayer = player;
				closestDistance = distance;
			}
		}

		return closestPlayer;
	}

	private void SyncState()
	{
		if( isServer )
		{
			_state = new EnemyState() { position = transform.position, rotation = 0.0f };
			_state.rotation = transform.rotation.eulerAngles.y;
		}
		else
		{
			// if we're too far away from where we should be, just teleport
			var positionDifference = Vector3.Distance( _state.position, transform.position );
			if( positionDifference > POSITION_THRESHOLD )
			{
				transform.position = _state.position;
			}
			else
			{
				transform.position = Vector3.Lerp( transform.position, _state.position, 0.1f );
			}

			var curAngle = transform.rotation.eulerAngles.y;
			transform.rotation = Quaternion.Euler( 0, Mathf.LerpAngle( curAngle, _state.rotation, 0.1f ), 0 );
		}
	}
}
