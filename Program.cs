using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Date = int;


using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Data;
using System.Security.Cryptography.X509Certificates;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        static Pod ToPod(string podProperties)
        {
            Pod pod;
            List<int> properties = new();
            int currValue = 0;
            foreach (char val in podProperties)
            {
                if (val == ' ')
                {
                    properties.Add(currValue);
                    currValue = 0;
                }
                else
                {
                    currValue += val + 10*currValue;
                }
            }
            pod.id = properties[0];
            pod.numStops = properties[1];
            int[] stops = new int[pod.numStops];
            for (int i = 2; i < pod.numStops + 2; i++)
            {
                stops[i-2] = properties[i];
            }
            pod.stops = stops;
            return pod;
        }

        static Building ToBuilding(string buildingProperties)
        {
            Building building;
            List<int> properties = new();
            int currValue = 0;
            foreach (char val in buildingProperties)
            {
                if (val == ' ')
                {
                    properties.Add(currValue);
                    currValue = 0;
                }
                else
                {
                    currValue += val + 10*currValue;
                }
            }
            building.type = properties[0];
            building.id = properties[1];
            building.X = properties[2];
            building.Y = properties[3];
            if (building.type == 0)
            {
                building.numAstronauts = properties[4];
                building.astronauts = [];
                for (int i = 5; i < building.numAstronauts + 5; i++)
                {
                    if (building.astronauts.ContainsKey(properties[i]))
                    {
                        building.astronauts[properties[i]]++;
                    }
                    else
                    {
                        building.astronauts[properties[i]] = 0;
                    }
                }
            }
            else
            {
                building.astronauts = [];
                building.numAstronauts = 0;
            }
            return building;
        }

        //before the game
        GameState currentState = new(0, 0, 0);

        // game loop
        while (true)
        {
            int resources = 3500;
            int numTravelRoutes = 2;
            TravelRoute[] travelRoutes = new TravelRoute[numTravelRoutes];
            for (int i = 0; i < numTravelRoutes; i++)
            {
                int buildingId1 = 0;
                int buildingId2 = 1;
                int capacity = 3;
                currentState.routeGraph[buildingId1].Add(buildingId2);
                currentState.routeGraph[buildingId2].Add(buildingId1);
                TravelRoute travelRoute;
                travelRoute.capacity = capacity;
                travelRoute.buildingId1 = buildingId1;
                travelRoute.buildingId2 = buildingId2;
            }

            int numPods = 1;
            Pod[] pods = new Pod[numPods];
            for (int i = 0; i < numPods; i++)
            {
                string podProperties = "100 5 1 0 1 0 1"; // podId numStops stop1 stop2 ...
                pods[i] = ToPod(podProperties);
            }

            int numNewBuildings = 2;
            Dictionary<int,Building> buildings = [];
            for (int i = 0; i < numNewBuildings; i++)
            {
                string buildingProperties = "2 3 95 38"; // type buildingId coordX coordY || 0 buildingId coordX coordY numAstronauts astronautType1 astronautType2 ...
                Building building = ToBuilding(buildingProperties);
                currentState.buildings[building.id] = building;
            }

            currentState.resources += resources;
            currentState.pods = pods;
            currentState.travelRoutes = travelRoutes;

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            Console.WriteLine("TUBE 0 1;TUBE 0 2;POD 42 0 1 0 2 0 1 0 2"); // TUBE | UPGRADE | TELEPORT | POD | DESTROY | WAIT
        }
    }
}


public struct GameDate(int day, int month)
{
    public int day = day;
    public int month = month;
}

public struct Building
{
    public int type, id, X, Y, numAstronauts;
    public Dictionary<int,int> astronauts; //key: astronautType, value num of astronauts this type
}

public struct Pod
{
    public int id, numStops;
    public int[] stops;
}

public struct TravelRoute
{
    public int buildingId1, buildingId2, capacity;
}

public struct Coord(int X, int Y)
{
    public int X = X;
    public int Y = Y;
}

public class Line(Coord coord1, Coord coord2)
{
    public Coord coord1 = coord1;
    public Coord coord2 = coord2;
    public bool IntersectsWith(Line line)
    {
        //TODO figure that out...
        return false;
    }
}

public class GameState
{
    public int points, resources;
    public int numBuildings, numPods, numTravelRoutes;
    public GameDate date;
    public Dictionary<int,Building> buildings;
    public Pod[] pods;
    public TravelRoute[] travelRoutes;
    public Dictionary<int,List<int>> routeGraph;
    public GameState(int numBuildings, int numPods, int numTravelRoutes)
    {
        points = 0;
        this.numBuildings = numBuildings;
        this.numPods = numPods;
        this.numTravelRoutes = numTravelRoutes;
        this.routeGraph = [];
        GameDate date;
        date.day = 0;
        date.month = 0;
        this.date = date;
        buildings = [];
        pods = new Pod[numPods];
        travelRoutes = new TravelRoute[numTravelRoutes];
    }

    public bool CanConstruct(int x1, int y1, int x2, int y2)
    {
        foreach (TravelRoute travelRoute in this.travelRoutes)
        {
            Coord coord1 = new(x1, y1);
            Coord coord2 = new(x2, y2);
            Coord coord3 = new(this.buildings[travelRoute.buildingId1].X, this.buildings[travelRoute.buildingId1].Y);
            Coord coord4 = new(this.buildings[travelRoute.buildingId2].X, this.buildings[travelRoute.buildingId2].Y);
            Line line1 = new(coord1, coord2);
            Line line2 = new(coord3, coord4);
            if (line1.IntersectsWith(line2))
            {
                return false;
            }
        }
        return true;
    }

    public void DistanceSort(List<int> buildingIds, int fromBuildingId)
    {
        int _Distance(int buildingId1, int buildingId2)
        {
            int dist1 = Math.Abs(this.buildings[fromBuildingId].X - this.buildings[buildingId1].X) + Math.Abs(this.buildings[fromBuildingId].Y - this.buildings[buildingId1].Y);
            int dist2 = Math.Abs(this.buildings[fromBuildingId].X - this.buildings[buildingId2].X) + Math.Abs(this.buildings[fromBuildingId].Y - this.buildings[buildingId2].Y);
            return dist1 - dist2;
        }
        buildingIds.Sort(_Distance);
    }

    public int GraphDistance(int buildingId1, int buildingId2, List<int> alreadyVisited)
    {
        int _GraphDistance(int fromBuildingId, int toBuildingId)
        {
            //TODO make the distance 0 for teleporters
            alreadyVisited.Add(fromBuildingId);
            if (fromBuildingId == toBuildingId)
            {
                return 0;
            }
            else
            {
                List<int> toVisit = [];
                foreach (int buildingId in this.routeGraph[fromBuildingId])
                {
                    if (!alreadyVisited.Contains(buildingId))
                    {
                        toVisit.Add(buildingId);
                    }
                }
                if (toVisit.Count == 0)
                {
                    return 10000;
                }
                else
                {
                    return toVisit.Select(x => 1 + _GraphDistance(x, toBuildingId)).Min();
                }
            }
        }
        return _GraphDistance(buildingId1, buildingId2);
    }

    public List<int> TubePath(int astronautType, int fromBuildingId)
    {
        List<int> rightTypeBuildingIds = [];
        List<int> path = [fromBuildingId];
        foreach (int toBuildingId in this.buildings.Keys)
        {
            if (this.buildings[toBuildingId].type == astronautType)
            {
                rightTypeBuildingIds.Add(toBuildingId);
            }
        }
        this.DistanceSort(rightTypeBuildingIds, fromBuildingId);
        foreach (int toBuildingId in rightTypeBuildingIds)
        {
            if (CanConstruct(buildings[fromBuildingId].X, buildings[fromBuildingId].Y, buildings[toBuildingId].X, buildings[toBuildingId].Y))
            {
                path.Add(toBuildingId);
                break;
            }
        }
        if (path.Count < 2)
        {
            //add the neighbours with the minimum distance from toBuildingId to path and continue...
        }
        return path;
    }
}