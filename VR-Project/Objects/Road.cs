using System;

public class Road 
{
	public string id;
	public RoadData data;

	public Road(string id, string route)
	{
		this.id = id;
		this.data = new RoadData(route);
	}

	public void SetRoute(string route)
    {
		this.data.route = route;
    }

	public void SetDiffuse(string diffuse)
	{
		this.data.diffuse = diffuse;
	}

	public void SetNormal(string normal)
	{
		this.data.normal = normal;
	}

	public void SetSpecular(string specular)
	{
		this.data.specular = specular;
	}

	public void SetHeightOffSet(double heightoffset)
	{
		this.data.heightoffset = heightoffset;
	}

	public class RoadData
    {
		public RoadData(string route)
        {
			this.route = route;
        }

		public string route { get; set; }

		#nullable enable
		public string? diffuse { get; set; }
		public string? normal { get; set; }
		public string? specular { get; set; }
		public double? heightoffset { get; set; }
	}

}
