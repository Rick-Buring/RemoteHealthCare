using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project.Objects.Route
{
    class DataGenerator
    {
        public List<Node> nodes;
        
        public DataGenerator()
        {
            this.nodes = new List<Node>();
        }

        public void addNode(float[] pos, float[] dir)
        {
            nodes.Add(new Node(pos, dir));
        }

        //the dataset for adding a path
        public class addData : Data
        {
            public Node[] nodes;
            public bool show;

            public addData(Node[] nodes, bool show)
            {
                this.nodes = nodes;
                this.show = show;
            }
        }
        
        public Data getDataAdd(bool show)
        {
            return new addData(nodes.ToArray(), show);
        }

        public class UpdateData : Data
        {
            public string uuid;
            public Node[] nodes;
            public bool show;

            public UpdateData(Node[] nodes, bool show, string uuid)
            {
                this.nodes = nodes;
                this.show = show;
                this.uuid = uuid;
            }
        }

        public Data getDataUpdate(bool show, string uuid)
        {
            return new UpdateData(nodes.ToArray(), show, uuid);
        }
    }
}
