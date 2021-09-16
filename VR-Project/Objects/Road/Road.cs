using System;

public class Road 
{
    public string id { get; set; }
    public RoadData data { get; set; }

    public class RoadData
    {
        public string route { get; set; }
        public string diffuse { get; set; }
        public string normal { get; set; }
        public string specular { get; set; }
        public double heightoffset { get; set; }
    }
}
