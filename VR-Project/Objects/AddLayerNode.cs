using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project
{
    class AddLayerNode
    {
        public string id;
        public LayerData data;

        public AddLayerNode(string id, string uuid, string texture)
        {
            this.id = id;
            this.data = new LayerData(uuid, texture);
        }

        public class LayerData
        {

            public string id { get; set; }
            public string diffuse { get; set; }
            public string normal { get; set; }
            public int minHeight { get; set; }
            public int maxHeight { get; set; }
            public int fadeDist { get; set; }

            public LayerData(string uuid, string texture)
            {
                this.id = uuid;
                this.diffuse = texture;
                this.normal = texture;
                this.minHeight = 0;
                this.maxHeight = 20;
                this.fadeDist = 1;
            }
        }

    }
}
