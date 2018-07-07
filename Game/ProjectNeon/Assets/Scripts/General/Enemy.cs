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
	private const float INTERPOLATE_THRESOLD = 2.0f;

	private GameObject[] _players;
	private GameObject _target;
	private NavMeshAgent _navMeshAgent;
	private TextMesh _healthText;
	private float _targetConsiderationDelay;
	private Vector3 _color;
	
	[SyncVar]
	private EnemyState _state;

	[SyncVar(hook = "HealthChangedOnServer")]
	private float _health;

	private void Start()
	{
		if( isServer )
		{
			_players = GameObject.FindGameObjectsWithTag( "Player" );

			_navMeshAgent = GetComponent<NavMeshAgent>();
			_targetConsiderationDelay = 0.0f;
		}

		_color = new Vector3( 0, 1, 1 );
		_health = 10;

		//_healthText = GetComponent<TextMesh>();
	}

	private void Awake()
	{
		_healthText = GetComponentInChildren<TextMesh>();
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
			var ray = new Ray( transform.position, direction.normalized );
			var rayHit = Physics.Raycast( ray, out hitInfo, distance, LayerMask.GetMask( "LevelGeometry" ) );
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
			if( positionDifference > INTERPOLATE_THRESOLD )
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

	// NOTE: This should only called by the server
	public void TakeDamage( Vector3 color, float damage )
	{
		var fractor = Vector3.Dot( color, _color );
		_health -= damage * fractor;

		if( _health <= 0.0f )
		{
			NetworkServer.Destroy( gameObject );
		}
	}

	private void HealthChangedOnServer( float newHealth )
	{
		var text = newHealth.ToString( "0.00" );
		_healthText.text = text;
	}
}
