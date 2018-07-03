using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	[SerializeField]
	public GameObject[] tiles;

	private System.Random _random;
	private TileParser _parser;

	private void Start()
	{
		_random = new System.Random();
		_parser = new TileParser();
		_parser.Parse( "area03.txt" );

		InstantiatePrefabs();
	}

	public void InstantiatePrefabs()
	{
		var roomIndex = _random.Next( 0, _parser.Rooms.Count );

		InstantiateWalls( roomIndex );
		InstantiateFloors( roomIndex );
	}

	public void InstantiateWalls( int roomIndex )
	{
		TileRoom room = _parser.Rooms[roomIndex];
		for( int curLayer = 0; curLayer < room.Layers.Count; curLayer++ )
		{
			TileLayer layer = room.Layers[curLayer];
			List<TileLayer> groupLayers = room.Layers.Where( x => x.Type == "wall" && !string.IsNullOrEmpty( x.Group ) && x.Group == layer.Group ).ToList();

			for( int curRow = 0; curRow < layer.Rows.Count; curRow++ )
			{
				TileRow row = layer.Rows[curRow];
				for( int curTile = 0; curTile < row.Tiles.Count; curTile++ )
				{
					string tmp = row.Tiles[curTile];
					if( _parser.WallAliases.ContainsKey( tmp ) )
					{
						WallAlias alias = _parser.WallAliases[tmp];

						var position = new Vector3( curTile * 10, 0, curRow * 10 );

						int sideIndex = alias.Parts[WallAlias.SIDE];
						int cornerIndex = alias.Parts[WallAlias.CORNER];
						int topIndex = alias.Parts[WallAlias.TOP];

						var left = ( curTile <= 0 );
						var right = ( curTile >= row.Tiles.Count - 1 );
						var front = ( curRow <= 0 );
						var back = ( curRow >= layer.Rows.Count - 1 );

						if( !left )
						{
							for( int i = 0; i < groupLayers.Count && !left; i++ )
							{
								TileLayer groupLayer = groupLayers[i];
								left = ( groupLayer.Rows[curRow].Tiles[curTile - 1] != "0" );
							}
						}

						if( !right )
						{
							for( int i = 0; i < groupLayers.Count && !right; i++ )
							{
								TileLayer groupLayer = groupLayers[i];
								right = ( groupLayer.Rows[curRow].Tiles[curTile + 1] != "0" );
							}
						}

						if( !front )
						{
							for( int i = 0; i < groupLayers.Count && !front; i++ )
							{
								TileLayer groupLayer = groupLayers[i];
								front = ( groupLayer.Rows[curRow - 1].Tiles[curTile] != "0" );
							}
						}

						if( !back )
						{
							for( int i = 0; i < groupLayers.Count && !back; i++ )
							{
								TileLayer groupLayer = groupLayers[i];
								back = ( groupLayer.Rows[curRow + 1].Tiles[curTile] != "0" );
							}
						}

						// instantiate sides
						if( sideIndex >= 0 )
						{
							var wallSide = tiles[sideIndex];

							if( !left )
								Instantiate( wallSide, position, Quaternion.Euler( 0, 180, 0 ) );
							if( !right )
								Instantiate( wallSide, position, Quaternion.Euler( 0, 0, 0 ) );
							if( !front )
								Instantiate( wallSide, position, Quaternion.Euler( 0, 90, 0 ) );
							if( !back )
								Instantiate( wallSide, position, Quaternion.Euler( 0, 270, 0 ) );
						}

						// instantiate corners
						if( cornerIndex >= 0 )
						{
							var wallCorner = tiles[cornerIndex];

							if( !left && !front )
								Instantiate( wallCorner, position, Quaternion.Euler( 0, 180, 0 ) );
							if( !right && !front )
								Instantiate( wallCorner, position, Quaternion.Euler( 0, 90, 0 ) );
							if( !left && !back )
								Instantiate( wallCorner, position, Quaternion.Euler( 0, -90, 0 ) );
							if( !right && !back )
								Instantiate( wallCorner, position, Quaternion.Euler( 0, 0, 0 ) );
						}

						// instantiate tops
						if( topIndex >= 0 )
							Instantiate( tiles[topIndex], position, Quaternion.identity );
					}
				}
			}
		}
	}

	public void InstantiateFloors( int roomIndex )
	{
		TileRoom room = _parser.Rooms[roomIndex];
		for( int curLayer = 0; curLayer < room.Layers.Count; curLayer++ )
		{
			TileLayer layer = room.Layers[curLayer];
			for( int curRow = 0; curRow < layer.Rows.Count; curRow++ )
			{
				TileRow row = layer.Rows[curRow];
				for( int curTile = 0; curTile < row.Tiles.Count; curTile++ )
				{
					string tmp = row.Tiles[curTile];
					if( _parser.FloorAliases.ContainsKey( tmp ) )
					{
						FloorAlias alias = _parser.FloorAliases[tmp];
						var position = new Vector3( curTile * 10, 0, curRow * 10 );

						int floorIndex = alias.Part;
						if( floorIndex >= 0 )
						{
							Instantiate( tiles[floorIndex], position, Quaternion.identity );
						}
					}
				}
			}
		}
	}
}
