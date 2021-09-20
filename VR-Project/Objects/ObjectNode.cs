using System;
using System.Collections.Generic;
using System.Text;

namespace VR_Project{ 
    class ObjectNode
    {

        public string id { get; set; }
        public Data data { get; set; }

        public ObjectNode(string id, string name, string file, int[] position)
        {
            this.id = id;
            this.data = new Data(name, file, position);
        }

         public class Data
        {
            public string name { get; set; }
            public Components? components { get; set; }
            public Data(string name, string file, int[] position)
            {
                this.name = name;

                this.components = new Components();
                this.components.transform = new Transform(position, 1, new int[3] { 0, 0, 0 });
                this.components.model = new Model(file);
            }

            public class Components
            {
                public Transform? transform { get; set; }
                public Model? model { get; set; }

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

                public Model(string file)
                {
                    this.file = file;
                    this.cullbackfaces = true;
                    this.animated = false;
                }
            }
        }

    }
}
