using System;

public class Skybox : VRObject
{

	public string id;
	public SkyboxData data { get; set; }
	
	public Skybox ()
    {
        this.data = new SkyboxData();
    }

    public void setID (string request)
    {
        this.id = request;
    }

    public void setData (double data)
    {
        this.data.time = data;
    }

    public class SkyboxData
    {
        public double time { get; set; }

    }
}
