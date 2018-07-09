using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
	[SerializeField]
	public float moveSpeed = 0.1f;

	[SerializeField]
	public float rushSpeed = 0.175f;

	[SerializeField]
	public Camera playerCamera;

	[SerializeField]
	public GameObject bulletPrefab;

	public Vector3 nameLabelRotation;

	[SyncVar( hook = "StateChangedOnServer" )]
	public PlayerState state;

	[SyncVar( hook = "PlayerNameChangedOnServer" )]
	public string playerName;

	private PlayerState _predictedState;
	private List<PlayerInput> _pendingInput;
	private float _prevNormalizedMouseX, _prevNormalizedMouseY;
	private TextMesh _nameLabel;

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
			_prevNormalizedMouseX = _prevNormalizedMouseY = 0.0f;
			Camera.main.gameObject.SetActive( false );

			Camera.Instantiate( playerCamera );
		}

		_nameLabel = GetComponentInChildren<TextMesh>();
		if( !string.IsNullOrEmpty( playerName ) )
			_nameLabel.text = playerName;
	}

	private void Update()
	{
		if( isLocalPlayer )
		{
			if( Input.GetMouseButtonDown( 0 ) )
			{
				CmdShootOnServer();
			}
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

		// reset the rotation of the name label
		_nameLabel.transform.rotation = Quaternion.Euler( nameLabelRotation );
	}

	private PlayerInput GetPlayerInput()
	{
		byte w = 0, a = 0, s = 0, d = 0, space = 0, lshift = 0;

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
		if( Input.GetKey( KeyCode.LeftShift ) )
			lshift = 1;

		var screenWidth = Screen.width;
		var screenHeight = Screen.height;

		var boundMouseX = Input.mousePosition.x;
		if( boundMouseX < 0 )
			boundMouseX = 0;
		else if( boundMouseX > screenWidth )
			boundMouseX = screenWidth;

		var boundMouseY = Input.mousePosition.y;
		if( boundMouseY < 0 )
			boundMouseY = 0;
		else if( boundMouseY > screenHeight )
			boundMouseY = screenHeight;

		var normalizedMouseX = boundMouseX / screenWidth;
		var normalizedMouseY = boundMouseY / screenHeight;

		normalizedMouseX *= 2.0f;
		normalizedMouseX -= 1.0f;

		normalizedMouseY *= 2.0f;
		normalizedMouseY -= 1.0f;

		var mouseX = normalizedMouseX;
		var mouseY = normalizedMouseY;

		PlayerInput result = null;
		if( w > 0 || a > 0 || s > 0 || d > 0 || space > 0 || lshift > 0 ||
			!Mathf.Approximately( mouseX, _prevNormalizedMouseX ) ||
			!Mathf.Approximately( mouseY, _prevNormalizedMouseY ) )
		{
			result = new PlayerInput()
			{
				w = w, a = a, s = s, d = d,
				space = space,
				lshift = lshift,
				mouseX = mouseX,
				mouseY = mouseY,
			};

			_prevNormalizedMouseX = mouseX;
			_prevNormalizedMouseY = mouseY;
		}
		
		return result;
	}

	private PlayerState ProcessPlayerInput( PlayerState prevState, PlayerInput input )
	{
		Vector3 newPosition = prevState.position;

		var currentSpeed = ( input.lshift > 0 ? rushSpeed : moveSpeed );

		var x = input.d - input.a;
		var z = input.w - input.s;
		newPosition += new Vector3( x, 0, z ).normalized * currentSpeed;
		
		var angle = Mathf.Atan2( input.mouseY, input.mouseX ) * Mathf.Rad2Deg;
		angle = -angle + 90.0f;

		var result = new PlayerState()
		{
			timestamp = prevState.timestamp + 1,
			position = newPosition,
			rotation = angle
		};

		return result;
	}

	private void SyncState()
	{
		if( isServer )
		{
			transform.position = state.position;
			transform.rotation = Quaternion.Euler(0, state.rotation, 0);
		}
		else
		{
			if( isLocalPlayer )
			{
				transform.position = _predictedState.position;
				transform.rotation = Quaternion.Euler( 0, _predictedState.rotation, 0 );
			}
			else
			{
				transform.position = Vector3.Lerp( transform.position, state.position, 0.1f );
				transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler( 0, state.rotation, 0 ), 0.1f );
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

	[Command]
	private void CmdShootOnServer()
	{
		var forward = (transform.rotation * Vector3.forward).normalized;

		var bullet = Instantiate( bulletPrefab, transform.position + forward*2.0f, Quaternion.identity );
		var bulletScript = bullet.GetComponent<Bullet>();
		bulletScript.Shoot( forward, Bullet.BULLET_DEFAULT_SPEED, 1 );

		NetworkServer.Spawn( bullet.gameObject );
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

	private void PlayerNameChangedOnServer( string newName )
	{
		playerName = newName;

		if( _nameLabel != null )
			_nameLabel.text = playerName;
	}
}
