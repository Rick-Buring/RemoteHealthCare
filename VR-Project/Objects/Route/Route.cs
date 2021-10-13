using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public RouteObject UpdateFollowSpeed(string uuid, float speed)
    {
        return new RouteObject("route/follow/speed", new FollowData(null, uuid, speed));

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

    public static async Task<Route> getRouteWithHeight (VR_Project.VrManager manager)
    {
        Route r = new Route();
        r.addPosAndDirection(new float[3] { 50, await manager.getTerrainHeight(new float[2] { 50 - 128, 50 - 128 }), 50 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 60, await manager.getTerrainHeight(new float[2] { 60 - 128, 40 - 128 }), 40 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 70, await manager.getTerrainHeight(new float[2] { 70 - 128, 35 - 128 }), 35 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 80, await manager.getTerrainHeight(new float[2] { 80 - 128, 30 - 128 }), 30 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 90, await manager.getTerrainHeight(new float[2] { 90 - 128, 28 - 128 }), 28 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 100, await manager.getTerrainHeight(new float[2] { 100 - 128, 25 - 128 }), 25 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 110, await manager.getTerrainHeight(new float[2] { 110 - 128, 30 - 128 }), 30 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 120, await manager.getTerrainHeight(new float[2] { 120 - 128, 35 - 128 }), 35 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 130, await manager.getTerrainHeight(new float[2] { 130 - 128, 45 - 128 }), 45 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 140, await manager.getTerrainHeight(new float[2] { 140 - 128, 60 - 128 }), 60 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 145, await manager.getTerrainHeight(new float[2] { 145 - 128, 70 - 128 }), 70 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 145, await manager.getTerrainHeight(new float[2] { 145 - 128, 73 - 128 }), 73 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 145, await manager.getTerrainHeight(new float[2] { 145 - 128, 76 - 128 }), 76 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 145, await manager.getTerrainHeight(new float[2] { 145 - 128, 80 - 128 }), 80 }, new float[3] { 0, 0, 0 });
        r.addPosAndDirection(new float[3] { 135, await manager.getTerrainHeight(new float[2] { 135 - 128, 90 - 128 }), 90 }, new float[3] { 0, 0, 0 });
        return r;
    }

}
