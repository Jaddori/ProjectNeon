using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class TileParser
{
	private Dictionary<string, WallAlias> _wallAliases;
	private Dictionary<string, FloorAlias> _floorAliases;
	private List<TileRoom> _rooms;
	private TileRoom _currentRoom;
	private TileLayer _currentLayer;
	
	public Dictionary<string, WallAlias> WallAliases { get { return _wallAliases; } }
	public Dictionary<string, FloorAlias> FloorAliases { get { return _floorAliases; } }
	public List<TileRoom> Rooms { get { return _rooms; } }

	public TileParser()
	{
		_wallAliases = new Dictionary<string, WallAlias>();
		_floorAliases = new Dictionary<string, FloorAlias>();
		_rooms = new List<TileRoom>();
	}

	public void Parse( string filename )
	{
		string fullPath = "ProjectNeon_Data/Resources/" + filename;
		if( !File.Exists( fullPath ) )
			fullPath = "bin/ProjectNeon/" + fullPath;

		var fileContent = File.ReadAllText( fullPath );
		var lines = fileContent.Split( new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries );

		var mode = string.Empty;
		var aliasType = string.Empty;

		for( int i = 0; i < lines.Length; i++ )
		{
			var line = lines[i].Trim();

			if( !String.IsNullOrEmpty( line ) && !line.StartsWith( "//" ) )
			{
				if( line.StartsWith( "alias" ) )
				{
					mode = "alias";
					var startIndex = line.IndexOf( '(' )+1;
					var endIndex = line.IndexOf( ')' );

					var args = line.Substring( startIndex, endIndex - startIndex );
					aliasType = args.Trim().ToLower();
				}
				else if( line == "room:" )
				{
					mode = line;
					CreateRoom();
				}
				else if( line.StartsWith( "layer" ) )
				{
					mode = "layer";
					ParseLayer( line );
				}
				else
				{
					switch( mode )
					{
						case "alias": ParseAlias( aliasType, line ); break;
						case "layer": ParseRow( line ); break;
					}
				}
			}
		}
	}

	private void CreateRoom()
	{
		_currentRoom = new TileRoom();
		_rooms.Add( _currentRoom );
	}

	private void ParseLayer( string line )
	{
		if( line.Contains( "(" ) && line.Contains( ")" ) )
		{
			string layerType = string.Empty;
			string group = string.Empty;

			int startIndex = line.IndexOf( '(' )+1;
			int endIndex = line.LastIndexOf( ')' );

			var args = line.Substring( startIndex, endIndex - startIndex );
			if( args.Contains( "," ) )
			{
				var argSplit = args.Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );
				layerType = argSplit[0].Trim();
				group = argSplit[1].Trim();
			}
			else
				layerType = args;

			_currentLayer = new TileLayer( layerType, group );
			_currentRoom.AddLayer( _currentLayer );
		}
	}

	private void ParseAlias( string type, string line )
	{
		if( type == "wall" )
		{
			Debug.Log( "Parsing wall alias" );

			var alias = new WallAlias();
			alias.Parse( line );

			_wallAliases.Add( alias.Name, alias );
		}
		else if( type == "floor" )
		{
			Debug.Log( "Parsing floor alias" );

			var alias = new FloorAlias();
			alias.Parse( line );

			_floorAliases.Add( alias.Name, alias );
		}
	}

	private void ParseRow( string line )
	{
		var split = line.Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );

		var row = new TileRow();
		foreach( var tile in split )
		{
			row.AddTile( tile );
		}

		_currentLayer.AddRow( row );
	}
}