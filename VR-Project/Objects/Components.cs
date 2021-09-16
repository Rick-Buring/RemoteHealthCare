using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VR_Project.Objects.IndividualComponents;

namespace VR_Project.Objects
{
    class Components
    {
        private Transform transform;

        public Components()
        {

        }

        public void addTransform(float[] pos, float scale, float[] dir)
        {
            transform = new Transform(pos, scale, dir);
        }
    }
}
