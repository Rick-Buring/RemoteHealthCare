using System;

public class Road 
{
    public string id { get; set; }
    public RoadData data { get; set; }

	public Road(string id, string route)
	{
		this.id = id;
		this.data = new RoadData(route);
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
			this.diffuse = "data/NetworkEngine/textures/tarmac_diffuse.png";
			this.normal = "data/NetworkEngine/textures/normale.png";
			this.specular = "data/NetworkEngine/textures/specular.png";
			this.heightoffset = 0.01;

		}

		public string route { get; set; }
		public string diffuse { get; set; }
		public string normal { get; set; }
		public string specular { get; set; }
		public double heightoffset { get; set; }
	}

}
