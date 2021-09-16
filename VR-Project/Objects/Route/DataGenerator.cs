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
        public string uuid {get; set;}
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

            public addData(Node[] nodes)
            {
                this.nodes = nodes;
            }
        }
        public Data getDataAdd()
        {
            Data addData = new addData(nodes.ToArray());
            return addData;
        }
    }
}
