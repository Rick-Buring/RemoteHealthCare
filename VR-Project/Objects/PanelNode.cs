using System;
using System.Collections.Generic;
using System.Text;

namespace VR_Project.Objects
{
    class PanelNode
    {

        public string id { get; set; }
        public Data data { get; set; }

        public PanelNode (string id, string name, string parent)
        {
            this.id = id;
            this.data = new Data(name, parent);
        }


        public class Data
        {
            public string name { get; set; }
            public string parent { get; set; }
            public Components components { get; set; }


            public Data (string name, string parent)
            {
                this.name = name;
                this.parent = parent;
                this.components = new Components();
            }

            public class Components
            {

                public Transform transform { get; set; }
                public Panel panel { get; set; }

                public Components ()
                {
                    this.transform = new Transform(new int[] { 0, 3, 0 }, 1, new int[] { 0, 0, 0 });
                    this.panel = new Panel(new int[] { 5,5 }, new int[] { 512, 512 }, new int[] { 1, 1, 1, 1 }, false);

                }


                public class Transform
                {
                    public int[] position { get; set; }
                    public int scale { get; set; }
                    public int[] rotation { get; set; }

                    public Transform(int[] position, int scale, int[] rotation)
                    {
                        this.position = position;
                        this.scale = scale;
                        this.rotation = rotation;
                    }
                }

                public class Panel
                {
                    public int[] size { get; set; }
                    public int[] resolution { get; set; }
                    public int[] background { get; set; }
                    public bool castShadow { get; set; }

                    public Panel (int[] size, int[] resolution, int[] background, bool castShadow)
                    {
                        this.size = size;
                        this.resolution = resolution;
                        this.background = background;
                        this.castShadow = castShadow;
                    }
                }

            }

        }

    }
}
