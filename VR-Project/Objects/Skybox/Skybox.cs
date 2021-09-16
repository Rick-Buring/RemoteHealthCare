using System;
using System.IO;
using System.Text.Json.Serialization;

public class Skybox 
{
    [JsonIgnore]
    private const string path = "data/NetworkEngine/textures/SkyBoxes";

    public string id;
	public SkyboxData data { get; set; }
    [JsonIgnore]
    private string type { get; set; }
    [JsonIgnore]
    private Files files { get; set; }
	
	public Skybox ()
    {
        this.data = new SkyboxData();
        this.files = new Files();
    }

    public void setType (SkyboxType type)
    {
        this.type = type.ToString().ToLower();
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

    public enum SkyboxType{
        STATIC,
        DYNAMIC
    }

    public class Files
    {
        public string xpos;
        public string xneg;
        public string ypos;
        public string yneg;
        public string zpos;
        public string zneg;

        public Files ()
        {
            this.xpos = Path.Combine(path, "interstellar/interstellar_rt.png");
            this.xneg = Path.Combine(path, "interstellar/interstellar_lf.png");
            this.ypos = Path.Combine(path, "interstellar/interstellar_up.png");
            this.yneg = Path.Combine(path, "interstellar/interstellar_dn.png");
            this.zpos = Path.Combine(path, "interstellar/interstellar_bk.png");
            this.zneg = Path.Combine(path, "interstellar/interstellar_ft.png");
        }
    }
}
