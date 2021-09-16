using System;
using System.Collections.Generic;

public class Nodes 
{
    public string id { get; set; }
    public NodeData data { get; set; }

    public class Transform
    {
        public List<int> position { get; set; }
        public int scale { get; set; }
        public List<int> rotation { get; set; }
    }

    public class Model
    {
        public string file { get; set; }
        public bool cullbackfaces { get; set; }
        public bool animated { get; set; }
        public string animation { get; set; }
    }

    public class Terrain
    {
        public bool smoothnormals { get; set; }
    }

    public class Panel
    {
        public List<int> size { get; set; }
        public List<int> resolution { get; set; }
        public List<int> background { get; set; }
        public bool castShadow { get; set; }
    }

    public class Water
    {
        public List<int> size { get; set; }
        public double resolution { get; set; }
    }

    public class Components
    {
        public Transform transform { get; set; }
        public Model model { get; set; }
        public Terrain terrain { get; set; }
        public Panel panel { get; set; }
        public Water water { get; set; }
    }

    public class NodeData
    {
        public string name { get; set; }
        public string parent { get; set; }
        public Components components { get; set; }
    }

}
