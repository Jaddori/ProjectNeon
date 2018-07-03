using System.Collections.Generic;

public class TileRoom
{
	private List<TileLayer> _layers;

	public List<TileLayer> Layers { get { return _layers; } }

	public TileRoom()
	{
		_layers = new List<TileLayer>();
	}

	public void AddLayer( TileLayer layer )
	{
		_layers.Add( layer );
	}
}

public class TileLayer
{
	private string _type;
	private string _group;
	private List<TileRow> _rows;

	public string Type { get { return _type; } set { _type = value; } }
	public string Group { get { return _group; } set { _group = value; } }
	public List<TileRow> Rows { get { return _rows; } }

	public TileLayer( string type )
	{
		_rows = new List<TileRow>();
		_type = type;
		_group = string.Empty;
	}

	public TileLayer( string type, string group )
	{
		_rows = new List<TileRow>();
		_type = type;
		_group = group;
	}

	public void AddRow( TileRow row )
	{
		_rows.Add( row );
	}
}

public class TileRow
{
	private List<string> _tiles;

	public List<string> Tiles { get { return _tiles; } }

	public TileRow()
	{
		_tiles = new List<string>();
	}

	public void AddTile( string tile )
	{
		_tiles.Add( tile );
	}
}