using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project.Objects.Route
{
    class Node
    {
        private float[] pos;
        private float[] dir;

        public Node(float[] pos, float[] dir)
        {
            this.pos = pos;
            this.dir = dir;
        }
    }
}
