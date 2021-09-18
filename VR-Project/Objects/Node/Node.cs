using System;
using System.Collections.Generic;

public class Node
{

    public string id { get; set; }
    public Data data { get; set; }

    
    public Node(string id, String name, bool smoothNormals)
    {
        this.id = id;
        Data nodeData = new Data();
        nodeData.components = new Components();
        nodeData.name = name;

        nodeData.components.transform = new Transform();
        nodeData.components.transform.position = new int[3] { 0, 0, 0 };
        nodeData.components.transform.scale = 1;
        nodeData.components.transform.rotation = new int[3] { 0, 0, 0 };

        nodeData.components.terrain = new Terrain();
       // nodeData.components.terrain.smoothnormals = smoothNormals;
        this.data = nodeData;
    }
    



    public class Transform
    {
        public int[]? position { get; set; }
        public int? scale { get; set; }
        public int[]? rotation { get; set; }
    }

    public class Model
    {
        public string? file { get; set; }
        public bool? cullbackfaces { get; set; }
        public bool? animated { get; set; }
        public string? animation { get; set; }
    }

    public class Terrain
    {
        public bool? smoothnormals { get; set; }
    }

    public class Panel
    {
        public int[]? size { get; set; }
        public int[]? resolution { get; set; }
        public int[]? background { get; set; }
        public bool? castShadow { get; set; }
    }

    public class Water
    {
        public int[]? size { get; set; }
        public double? resolution { get; set; }
    }

    public class Components
    {
        public Transform? transform { get; set; }
        public Model? model { get; set; }
        public Terrain? terrain { get; set; }
        public Panel? panel { get; set; }
        public Water? water { get; set; }
    }

    public class Data
    {
        public string name { get; set; }
        public string? parent { get; set; }
        public Components? components { get; set; }
    }

}
