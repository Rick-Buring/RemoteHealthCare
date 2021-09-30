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

        nodeData.components = new Data.Components();
        nodeData.name = name;

        nodeData.components.transform = new Data.Transform(new int[3] { 0, 0, 0 }, 1, new int[3] { 0, 0, 0 });


        nodeData.components.terrain = new Data.Terrain();
        // nodeData.components.terrain.smoothnormals = smoothNormals;
        /*
        nodeData.components.model = new Data.Model();
        nodeData.components.model.file = "data/NetworkEngine/textures/terrain/grass_autumn_red_d.jpg";
        nodeData.components.model.cullbackfaces = false;
        nodeData.components.model.animated = false;
        

        /*
        nodeData.components.panel = new Panel();
        nodeData.components.panel.size = new int[2] { 1, 1 };
        nodeData.components.panel.resolution = new int[2] { 512, 512 };
        nodeData.components.panel.background = new int[4] { 1, 1, 1, 1 };
        nodeData.components.panel.castShadow = true;
        */
        this.data = nodeData;
    }

    public Node(string id)
    {
        this.id = id;
        this.data = new Data();
    }

    public Node(string id, string name, string file, int[] position)
    {
        this.id = id;
        this.data = new Data(name, file, position);
    }

    public class Data
    {
      
        public string name { get; set; }
        public string? id { get; set; }
        public string? parent { get; set; }
        public string? stop { get; set; }
        public int[]? position { get; set; }
        public string? rotate { get; set; }
        public string? interpolate { get; set; }
        public string? followheight { get; set; }
        public int? speed { get; set; }
        public int? time { get; set; }
        public string? diffuse { get; set; }
        public string? normal { get; set; }
        public int? minHeight { get; set; }
        public int? maxheight { get; set; }
        public int? fadeDist { get; set; }

        public Data()
        {  }

            public Data(string name, string file, int[] position)
        { 
            this.name = name;

            this.components = new Components();
            this.components.transform = new Transform(position, 1, new int[3] { 0, 0, 0 });
            this.components.model = new Model(file);
        }

        public Components? components { get; set; }

        public class Components
        {
            public Transform? transform { get; set; }
            public Model? model { get; set; }
            public Terrain? terrain { get; set; }
            public Panel? panel { get; set; }
            public Water? water { get; set; }

        }

        public class Transform
        {
            public Transform(int[] position, int? scale, int[]? rotation)
            {
                this.position = position;
                this.scale = scale;
                this.rotation = rotation;
            }

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

            public Model (string file)
            {
                this.file = file;
                this.cullbackfaces = true;
                this.animated = false;
            }
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
    }



}
