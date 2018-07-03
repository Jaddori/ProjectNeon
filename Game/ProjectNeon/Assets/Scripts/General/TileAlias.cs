using UnityEngine;
using System.Collections;
using System;

public class WallAlias
{
	public const int SIDE = 0;
	public const int CORNER = SIDE + 1;
	public const int TOP = CORNER + 1;
	public const int PARTS = TOP + 1;

	private string _name;
	private int[] _parts;

	public string Name { get { return _name; } set { _name = value; } }
	public int[] Parts { get { return _parts; } set { _parts = value; } }

	public WallAlias()
	{
		_name = string.Empty;
		_parts = new int[PARTS] { 0, 0, 0 };
	}

	public WallAlias( string name )
	{
		_name = name;
		_parts = new int[PARTS] { 0, 0, 0 };
	}

	public void Parse( string line )
	{
		var split = line.Split( new[] { "=" }, StringSplitOptions.RemoveEmptyEntries );

		_name = split[0].Trim();

		var partSplit = split[1].Trim().Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );
		
		int len = partSplit.Length;
		for( int i = 0; i < len; i++ )
			_parts[i] = int.Parse( partSplit[i] ) - 1;
	}
}

public class FloorAlias
{
	private string _name;
	private int _part;

	public string Name { get { return _name; } set { _name = value; } }
	public int Part { get { return _part; } set { _part = value; } }

	public FloorAlias()
	{
		_name = string.Empty;
		_part = 0;
	}

	public FloorAlias( string name )
	{
		_name = name;
		_part = 0;
	}

	public void Parse( string line )
	{
		var split = line.Split( new[] { "=" }, StringSplitOptions.RemoveEmptyEntries );

		_name = split[0].Trim();
		_part = int.Parse( split[1].Trim() );
	}
}
