using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
	[SerializeField]
	public float moveSpeed = 0.1f;

	[SerializeField]
	public Camera playerCamera;

	[SyncVar( hook = "StateChangedOnServer" )]
	public PlayerState state;

	private PlayerState _predictedState;
	private List<PlayerInput> _pendingInput;

	private void Awake()
	{
		state = new PlayerState()
		{
			timestamp = 0,
			position = new Vector3( 0, 1, 0 ),
			rotation = 0.0f
		};
	}

	private void Start()
	{
		if( isLocalPlayer )
		{
			_pendingInput = new List<PlayerInput>();
			Camera.main.gameObject.SetActive( false );

			Camera.Instantiate( playerCamera );
		}
	}

	private void FixedUpdate()
	{
		if( isLocalPlayer )
		{
			var input = GetPlayerInput();
			if( input != null )
			{
				_pendingInput.Add( input );
				UpdatePredictedState();
				CmdMoveOnServer( input );
			}
		}

		SyncState();
	}

	private PlayerInput GetPlayerInput()
	{
		byte w = 0, a = 0, s = 0, d = 0, space = 0;

		if( Input.GetKey( KeyCode.W ) )
			w = 1;
		if( Input.GetKey( KeyCode.A ) )
			a = 1;
		if( Input.GetKey( KeyCode.S ) )
			s = 1;
		if( Input.GetKey( KeyCode.D ) )
			d = 1;

		if( Input.GetKey( KeyCode.Space ) )
			space = 1;

		PlayerInput result = null;
		if( w > 0 || a > 0 || s > 0 || d > 0 || space > 0 )
			result = new PlayerInput() { w = w, a = a, s = s, d = d, space = space };

		return result;
	}

	private PlayerState ProcessPlayerInput( PlayerState prevState, PlayerInput input )
	{
		Vector3 newPosition = prevState.position;

		var x = input.d - input.a;
		var z = input.w - input.s;
		Vector3 movement = new Vector3( x, 0, z ).normalized * moveSpeed;

		Quaternion rotation = transform.rotation;
		newPosition += rotation * Vector3.right * movement.x;
		newPosition += rotation * Vector3.forward * movement.z;

		var result = new PlayerState()
		{
			timestamp = prevState.timestamp + 1,
			position = newPosition,
			rotation = prevState.rotation
		};

		return result;
	}

	private void SyncState()
	{
		if( isServer )
		{
			transform.position = state.position;
		}
		else
		{
			if( isLocalPlayer )
			{
				transform.position = _predictedState.position;
			}
			else
			{
				transform.position = Vector3.Lerp( transform.position, state.position, 0.1f );
			}
		}
	}

	private void UpdatePredictedState()
	{
		_predictedState = state;

		foreach( var input in _pendingInput )
			_predictedState = ProcessPlayerInput( _predictedState, input );
	}

	[Command]
	private void CmdMoveOnServer( PlayerInput input )
	{
		state = ProcessPlayerInput( state, input );
	}

	private void StateChangedOnServer( PlayerState newState )
	{
		state = newState;

		if( _pendingInput != null )
		{
			var len = _predictedState.timestamp - state.timestamp;
			while( _pendingInput.Count > len )
				_pendingInput.RemoveAt( 0 );

			UpdatePredictedState();
		}
	}
}
