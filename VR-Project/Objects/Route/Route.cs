using System;
using System.Collections.Generic;
using VR_Project.Objects.Route;

public class Route
{
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
    public RouteObject addRoute(bool show)
    {
        return new RouteObject("route/add", dataGen.getDataAdd(show));
    }

    
    //sets the id to update the route, and sets data
    public RouteObject updateRoute(string uuid, bool show)
    {
        return new RouteObject("route/update", dataGen.getDataUpdate(show, uuid));
    }


    public RouteObject followRoute(string routeid, string nodeid, float speed)
    {
        return new RouteObject("route/follow", new FollowData(routeid, nodeid, speed));
    }

    public class RouteObject
    {
        public string id;
        public Data data;
        

        public RouteObject(string id, Data data)
        {
            this.id = id;
            this.data = data;
        }
    }

    public static Route getRoute ()
    {
        Route r = new Route();

        r.addPosAndDirection(new float[3] { 0, 0, 0 }, new float[3] { 0, 0, 10 });
        r.addPosAndDirection(new float[3] { 0, 0, 10 }, new float[3] { 0, 0, 20 });
        r.addPosAndDirection(new float[3] { 0, 0, 20 }, new float[3] { 10, 0, 20 });
        r.addPosAndDirection(new float[3] { 10, 0, 20 }, new float[3] { 20, 0, 20 });
        r.addPosAndDirection(new float[3] { 20, 0, 20 }, new float[3] { 20, 0, 10 });
        r.addPosAndDirection(new float[3] { 20, 0, 10 }, new float[3] { 30, 0, 10 });
        r.addPosAndDirection(new float[3] { 30, 0, 10 }, new float[3] { 20, 0, 0 });
        r.addPosAndDirection(new float[3] { 20, 0, 0 }, new float[3] { 10, 0, 0 });
        r.addPosAndDirection(new float[3] { 10, 0, 0 }, new float[3] { 0, 0, 0 });

        return r;
    }
}
