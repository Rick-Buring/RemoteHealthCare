using System;
using VR_Project.Objects;

public class Nodes : VRObject
{
    private string name;
    private Components components;

    //transform
    private float[] pos;
    private float scale;
    private float[] dir;
	public Nodes(string name)
	{
        this.name = name;
        this.components = new Components();
	}

    public void setTransform(float[] pos, float scale, float[] dir)
    {
        if (pos.Length == 3 || dir.Length == 3)
        {
            components.addTransform(pos, scale, dir);
        }
    }
    public void addNode()
    {
        this.id = "scene/node/add";
        this.data = 
    }


    public override byte[] getByte()
    {
        throw new NotImplementedException();
    }
}
