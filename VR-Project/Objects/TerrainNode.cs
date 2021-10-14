using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project
{
    
    class TerrainNode
    {

        public string id;
        public TerrainNodeData data;

        public TerrainNode(string id, string name, bool smoothNormals)
        {
            this.id = id;
            this.data = new TerrainNodeData(name, smoothNormals);
        }

        public class TerrainNodeData
        {
            public string name { get; set; }
#nullable enable
            public Components? components { get; set; }
#nullable disable

            public TerrainNodeData(string name, bool smoothNormals)
            {
                this.name = name;
                this.components = new Components(smoothNormals);
            }


            public class Components
            {
#nullable enable
                public Transform? transform { get; set; }
                public Terrain? terrain { get; set; }
#nullable disable
                public Components(bool smoothNormals)
                {
                    this.transform = new Transform(new int[3] { 0, 0, 0 }, 1, new int[3] { 0, 0, 0 });
                    this.terrain = new Terrain
                    {
                        smoothnormals = smoothNormals
                    };
                }

                public class Terrain
                {
                    public bool? smoothnormals { get; set; }
                }

                public class Transform
                {
                    public Transform(int[] position, int? scale, int[] rotation)
                    {
                        this.position = position;
                        this.scale = scale;
                        this.rotation = rotation;
                    }
#nullable enable
                    public int[]? position { get; set; }
                    public int? scale { get; set; }
                    public int[]? rotation { get; set; }
#nullable disable
                }
                
            }
        }
    }
}
