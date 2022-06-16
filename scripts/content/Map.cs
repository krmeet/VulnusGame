using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using File = Godot.File;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class Map
{
	public static int LatestFormat = 2;
	[JsonProperty("_version")]
	public int FormatVersion;
	[NonSerialized]
	public string Path;
	[JsonProperty("_artist")]
	public string Artist;
	[JsonProperty("_title")]
	public string Title;
	[JsonProperty("_difficulties")]
	public List<string> _difficulties;
	public List<Difficulty> Difficulties;
	[JsonProperty("_mappers")]
	public List<string> Mappers;
	[JsonProperty("_music")]
	public string Music;
	[Serializable, JsonObject]
	public class Note
	{
		public float X;
		public float Y;
		public float T;
	}
	[Serializable, JsonObject(MemberSerialization.OptIn)]
	public class Difficulty
	{
		[JsonProperty("_name")]
		public string Name;
		[JsonIgnore]
		public string Path;
		[NonSerialized, JsonIgnore]
		public MapData Data;
		public void Load(Map map)
		{
			if (Data != null) return;
			var file = new File();
			file.Open(map.Path + "/" + Path, File.ModeFlags.Read);
			var data = JsonConvert.DeserializeObject<MapData>(file.GetAsText());
			this.Data = data;
			file.Close();
		}
	}
	[JsonObject]
	public class MapData : Difficulty
	{
		[JsonProperty("_notes")]
		public List<Note> Notes;
	}
	public static Map LoadCached(string path)
	{
		var file = new File();
		if (file.Open(path.PlusFile("cache.bin"), File.ModeFlags.Read) == Error.Ok)
		{
			var deserializer = new BinaryFormatter();
			var buffer = file.GetBuffer((long)file.GetLen());
			var stream = new MemoryStream(buffer);
			var cachedMap = (Map)deserializer.Deserialize(stream);
			return cachedMap;
		}
		file.Open(path, File.ModeFlags.Read);
		var map = JsonConvert.DeserializeObject<Map>(file.GetAsText());
		file.Close();
		return map;
	}
	public void Serialize(Stream stream)
	{
		var serializer = new BinaryFormatter();
		serializer.Serialize(stream, this);
	}
}