using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
	public const float BULLET_DEFAULT_SPEED = 5.0f;
	public const float INTERPOLATE_THRESOLD = 2.0f;

	[SyncVar]
	private Vector3 statePosition;

	private Rigidbody _rigidbody;
	private Vector3 _direction;
	private float _speed;
	private Vector3 _color;
	private float _damage;
	private int _enemyLayer;

	public Vector3 Color { get { return _color; } set { _color = value; } }
	public float Damage { get { return _damage; } set { _damage = value; } }

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_enemyLayer = LayerMask.NameToLayer( "Enemies" );
		_color = new Vector3( 0, 0, 1 );
	}

	private void FixedUpdate()
	{
		if( isServer )
		{
			statePosition = transform.position;
		}
		else
		{
			var distance = Vector3.Distance( transform.position, statePosition );
			if( distance > INTERPOLATE_THRESOLD )
				transform.position = statePosition;
			else
				transform.position = Vector3.Lerp( transform.position, statePosition, 0.1f );
		}
	}

	public void Shoot( Vector3 direction, float speed, float damage )
	{
		_rigidbody.AddForce( direction * speed, ForceMode.Impulse );
		_damage = damage;
	}

	private void OnCollisionEnter( Collision collision )
	{
		if( isServer )
		{
			if( collision.gameObject.layer == _enemyLayer )
			{
				var enemy = collision.gameObject;
				var enemyScript = enemy.GetComponent<Enemy>();

				enemyScript.TakeDamage( _color, _damage );
			}

			NetworkServer.Destroy( gameObject );
		}
	}
}
