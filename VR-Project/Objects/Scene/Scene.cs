using System;

public class Scene
{
	public string id;
	public SceneData data;

	public Scene(string id)
	{
		this.id = id;
		this.data = new SceneData();
	}

	public void setFileName(string fileName)
	{
		this.data.filename = fileName;
	}

	public void setOverwrite(bool overwrite)
	{
		this.data.overwrite = overwrite;
	}

	public class SceneData
	{
#nullable enable
		public string? filename { get; set; }
		public bool? overwrite { get; set; }
	}
}
