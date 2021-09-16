using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project.Objects.IndividualComponents
{
    class Transform
    {
        private float[] pos;
        private float scale;
        private float[] dir;

        public Transform(float[] pos, float scale, float[] dir)
        {
            this.pos = pos;
            this.scale = scale;
            this.dir = dir;
        }
    }
}
