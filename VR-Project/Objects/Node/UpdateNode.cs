using System;
using System.Collections.Generic;
using System.Text;

namespace VR_Project.Objects.Node
{
    class UpdateNode
    {

        public string id { get; set; }
        public Data data { get; set; }

        public UpdateNode(string id, string selfID, string parent, double[] rotation)
        {
            this.id = id;
            this.data = new Data(selfID, parent, rotation);
        }

        public class Data
        {
            public string id { get; set; }
            public string parent { get; set; }
            public Transform transform { get; set; }

            public Data (string id, string parent, double[] rotation)
            {
                this.id = id;
                this.parent = parent;
                this.transform = new Transform(rotation);
            }

            public class Transform
            {
                public float[] position;
                public double scale;
                public double[] rotation;

                public Transform (double[] rotation)
                {
                    this.position = new float[] { 0, 0, 0};
                    this.scale = 1;
                    this.rotation = rotation;
                }
            }
        }

    }
}
