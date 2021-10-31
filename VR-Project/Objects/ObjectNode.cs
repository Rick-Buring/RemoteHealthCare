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

        public ObjectNode(string id, string name, string file, int[] position, string animationFile)
        {
            this.id = id;
            this.data = new Data(name, file, position);
        }

        public class Data
        {
            public string name { get; set; }
#nullable enable
            public Components? components { get; set; }
#nullable disable
            public Data(string name, string file, int[] position)
            {
                this.name = name;

                this.components = new Components
                {
                    transform = new Transform(position, 1, new double[3] { 0, 0, 0 }),
                    model = new Model(file)
                };
            }

            public Data(string name, string file, int[] position, string animationFile)
            {
                this.name = name;

                this.components = new Components
                {
                    transform = new Transform(position, 1, new double[3] { 0, 0, 0 }),
                    model = new Model(file, animationFile)
                };
            }

            public class Components
            {
#nullable enable
                public Transform? transform { get; set; }
                public Model? model { get; set; }
#nullable disable
            }

            public class Transform
            {
#nullable enable
                public Transform(int[] position, int? scale, double[]? rotation)
                {
                    this.position = position;
                    this.scale = scale;
                    this.rotation = rotation;
                }

                public int[]? position { get; set; }
                public int? scale { get; set; }
                public double[]? rotation { get; set; }
#nullable disable
            }

            public class Model
            {
#nullable enable
                public string? file { get; set; }
                public bool? cullbackfaces { get; set; }
                public bool? animated { get; set; }
                public string? animation { get; set; }
#nullable disable

                public Model(string file)
                {
                    this.file = file;
                    this.cullbackfaces = true;
                    this.animated = false;
                }

                public Model(string file, string animationFile)
                {
                    this.file = file;
                    this.cullbackfaces = true;
                    this.animated = true;
                    this.animation = animationFile;
                }
            }
        }

    }
}
