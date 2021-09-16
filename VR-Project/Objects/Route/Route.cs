using System;
using System.Collections.Generic;
using VR_Project.Objects.Route;

public class Route
{
    private string id{get; set;}
    private object data {get; set;}
    private DataGenerator dataGen;

    //makes the Route object but doesnt do anything with it yet
    public Route()
	{
        this.dataGen = new DataGenerator();
	}

    //add positions and directions 1 by 1, these positions wil later form the path
    public void addPosAndDirection(float[] pos, float[] dir)
    {
        if (pos.Length == 3 || dir.Length == 3)
        {
            dataGen.addNode(pos, dir);
        }
    }

    //sets the id to add route, and sets data
    public void addRoute()
    {
        this.id = "route/add";
        this.data = dataGen.getDataAdd();
    }

    //sets the id to update the route, and sets data
    public void updateRoute(string uuid)
    {
        //add to data
    }
}
