using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project.Objects.Route
{
    class FollowData : Data
    {
        public string route { get; set; }
        public string node { get; set; }
        public float speed { get; set; }
        public float offset { get; set; }
        public string rotate { get; set; }
        public float smoothing { get; set; }
        public bool followHeight { get; set; }
        public float[] rotateOffset { get; set; }
        public float[] positionOffset { get; set; }

        public FollowData(string routeid, string nodeid, float speed)
        {
            this.route = routeid;
            this.node = nodeid;
            this.speed = speed;
            this.offset = 0.0f;
            this.rotate = "XZ";
            this.smoothing = 1.0f;
            this.followHeight = false;
            this.rotateOffset = new float[3] { 0, 0, 0 };
            this.positionOffset = new float[3] { 0, 0, 0 };
        }
    }
}
