using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletRay : MonoBehaviour
{
	public float maxAlpha = 0.5f;
	public float minAlpha = 0.0f;

	private LineRenderer _lineRenderer;
	private float _lifetime;
	private float _elapsed;
	private Color _color;
	private Vector3 _end;

	private void Start()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.SetPosition( 1, _end );
	}

	public void Spawn( Vector3 start, Vector3 end, float lifetime, Color color )
	{
		transform.position = start;
		_end = end - start;

		_lifetime = lifetime;
		_elapsed = 0.0001f;
		_color = color;
	}

	public void Update()
	{
		if( _elapsed < _lifetime )
		{
			_elapsed += Time.deltaTime;
			
			var startColor = _color;
			var endColor = _color;

			var t = ( _elapsed / _lifetime );
			endColor.a = Mathf.Lerp( maxAlpha, minAlpha, t );
			startColor.a = endColor.a * 0.5f;
			
			_lineRenderer.startColor = startColor;
			_lineRenderer.endColor = endColor;
		}
		else
		{
			Destroy( gameObject );
		}
	}
}
