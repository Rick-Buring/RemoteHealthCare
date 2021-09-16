using System;
using System.Collections.Generic;

public class Route : VRObject 
{
    private Dictionary<float[], float[]> positions;

    //makes the Route object but doesnt do anything with it yet
    public Route()
	{
        this.positions = new Dictionary<float[], float[]>();
	}

    //add positions and directions 1 by 1, these positions wil later form the path
    public void addPosAnddirection(float[] pos, float[] dir)
    {
        if (pos.Length == 3 || dir.Length == 3)
        {
            positions[pos] = dir;
        }
    }

    //sets the id to add route, and sets data
    public void addRoute()
    {
        this.id = "route/add";
        setData();
    }

    //sets the id to update the route, and sets data
    public void updateRoute(string uuid)
    {
        if (uuid != null)
        {
            this.id = "route/update";
            setData(uuid);
        }
    }

    //sets the main data of the message
    private void setData(string uuid)
    {
        if(uuid != null)
        {
            //todo: add the uuid to data
        }

        //todo: add notes to data
    }


    private void setData()
    {
        setData(null);
    }


    public override byte[] getByte()
    {
        throw new NotImplementedException();
    }
}
