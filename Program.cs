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
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

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
            List<int> properties = [];
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
                    currValue = val - '0' + 10*currValue;
                }
            }
            properties.Add(currValue);
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
                    currValue = val - '0' + 10*currValue;
                }
            }
            properties.Add(currValue);
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
            currentState.routeGraph = [];
            int resources = 3500;
            int numTravelRoutes = 1;
            currentState.numTravelRoutes = numTravelRoutes;
            List<TravelRoute> travelRoutes = [];
            for (int i = 0; i < numTravelRoutes; i++)
            {
                int buildingId1 = 0;
                int buildingId2 = 1;
                int capacity = 3;
                currentState.AddTubeInGraph(buildingId1, buildingId2);
                TravelRoute travelRoute;
                travelRoute.capacity = capacity;
                travelRoute.buildingId1 = buildingId1;
                travelRoute.buildingId2 = buildingId2;
                travelRoutes.Add(travelRoute);
            }

            int numPods = 1;
            currentState.numPods = numPods;
            Pod[] pods = new Pod[numPods];
            for (int i = 0; i < numPods; i++)
            {
                string podProperties = "100 5 1 0 1 0 1"; // podId numStops stop1 stop2 ...
                pods[i] = ToPod(podProperties);
            }

            int numNewBuildings = 3;
            currentState.numBuildings += numNewBuildings;
            List<int> newBuildingIds = [];
            for (int i = 0; i < numNewBuildings; i++)
            {
                string buildingProperties;
                if (i == 0)
                {
                    buildingProperties = "0 0 80 60 30 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2 1 2"; // type buildingId coordX coordY || 0 buildingId coordX coordY numAstronauts astronautType1 astronautType2 ...
                }
                else if (i == 1)
                {
                    buildingProperties = "1 1 40 30";
                }
                else
                {
                    buildingProperties = "2 2 120 30";
                }
                Building building = ToBuilding(buildingProperties);
                currentState.buildings[building.id] = building;
                newBuildingIds.Add(building.id);
                if (building.type == 0)
                {
                    currentState.landingPadIds.Add(building.id);
                }
            }

            currentState.resources += resources;
            currentState.pods = pods;
            currentState.travelRoutes = travelRoutes;

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            string actions = currentState.ContrucTubesAndPods(currentState.NewTubes());
            if (actions == "")
            {
                Console.WriteLine("WAIT");
            }
            else
            {
                Console.WriteLine(actions); // TUBE | UPGRADE | TELEPORT | POD | DESTROY | WAIT
            }
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

public struct Pair<T>(T X, T Y)
{
    public T X = X;
    public T Y = Y;
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
    public List<TravelRoute> travelRoutes;
    public List<int> landingPadIds;
    public Dictionary<int,List<int>> routeGraph;
    public GameState(int numBuildings, int numPods, int numTravelRoutes)
    {
        points = 0;
        this.numBuildings = numBuildings;
        this.numPods = numPods;
        this.numTravelRoutes = numTravelRoutes;
        this.routeGraph = [];
        this.landingPadIds = [];
        GameDate date;
        date.day = 0;
        date.month = 0;
        this.date = date;
        buildings = [];
        pods = new Pod[numPods];
        travelRoutes = [];
    }
    
    public void AddTubeInGraph(int buildingId1, int buildingId2)
    {
        if (routeGraph.ContainsKey(buildingId1))
        {
            this.routeGraph[buildingId1].Add(buildingId2);
        }
        else
        {
            this.routeGraph[buildingId1] = [buildingId2];
        }
        if (routeGraph.ContainsKey(buildingId2))
        {
            this.routeGraph[buildingId2].Add(buildingId1);
        }
        else
        {
            this.routeGraph[buildingId2] = [buildingId1];
        }
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

    public int Distance(int buildingId1, int buildingId2)
    {
        return (int)Math.Floor(Math.Pow(Math.Sqrt(this.buildings[buildingId1].X - this.buildings[buildingId2].X) 
                                      + Math.Sqrt(this.buildings[buildingId1].Y - this.buildings[buildingId2].Y), 2));
    }

    public int GraphDistance(int buildingId1, int buildingId2, List<int> alreadyVisited)
    {
        bool foundOne = false;
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
                        foundOne = true;
                    }
                }
                if (toVisit.Count == 0 || foundOne)
                {
                    return 10000;
                }
                else
                {
                    //TODO distance 0 for teleporters
                    return toVisit.Select(x => 1 + _GraphDistance(x, toBuildingId)).Min();
                }
            }
        }
        return _GraphDistance(buildingId1, buildingId2);
    }

    public bool InConnexGraph(int fromBuildingId, int toBuildingId)
    {
        bool res = false;
        bool _InConnexGraph(int fromBuildingId, int toBuildingId)
        {
            List<bool> resList = [];
            if (!res && fromBuildingId != toBuildingId)
            {
                foreach (int buildingId in this.routeGraph[fromBuildingId])
                {
                    resList.Add(_InConnexGraph(fromBuildingId, toBuildingId));
                }
                return resList.Contains(true);
            }
            else
            {
                res = true;
                return true;
            }
        }
        return _InConnexGraph(fromBuildingId, toBuildingId);
    }

    public int DestinationForAstronaut(int astronautType, int fromBuildingId)
    {
        List<int> rightTypeBuildingIds = [];
        int destinationId = -1;
        foreach (int toBuildingId in this.buildings.Keys)
        {
            if (this.buildings[toBuildingId].type == astronautType)
            {
                rightTypeBuildingIds.Add(toBuildingId);
            }
        }
        //this.DistanceSort(rightTypeBuildingIds, fromBuildingId);
        foreach (int toBuildingId in rightTypeBuildingIds)
        {
            if (CanConstruct(buildings[fromBuildingId].X, buildings[fromBuildingId].Y, buildings[toBuildingId].X, buildings[toBuildingId].Y))
            {
                destinationId = toBuildingId;
                break;
            }
        }
        if (destinationId == -1)
        {
            foreach (int toBuildingId in this.buildings.Keys)
            {
                if (CanConstruct(buildings[fromBuildingId].X, buildings[fromBuildingId].Y, buildings[toBuildingId].X, buildings[toBuildingId].Y))
                {
                    foreach (int toRightTypeBuildingId in rightTypeBuildingIds)
                    {
                        if (InConnexGraph(toRightTypeBuildingId, toBuildingId))
                        {
                            destinationId = toBuildingId;
                        }
                    }
                }
            }
        }
        return destinationId;
    }

    public List<Pair<int>> NewTubes() // Modifies the routeGraph with the new tubes (checks every landing pad)
    {
        List<Pair<int>> newTubes = [];
        foreach (int landingPadId in this.landingPadIds)
        {
            foreach (int astronautType in this.buildings[landingPadId].astronauts.Keys)
            {
                Pair<int> newTube = new(landingPadId, DestinationForAstronaut(astronautType, landingPadId));
                newTubes.Add(newTube);
                this.AddTubeInGraph(newTube.Y, newTube.Y);
            }
        }
        return newTubes;
    }

    public string ContrucTubesAndPods(List<Pair<int>> tubes)
    {
        string res = "";
        string tubeString = "";
        string podString = "";
        foreach (Pair<int> tubeIds in tubes)
        {
            if (tubeIds.Y != -1)
            {
                tubeString = "TUBE " + tubeIds.X + ' '+ tubeIds.Y + ';';
                this.resources -= Distance(tubeIds.X, tubeIds.Y);
                podString = "POD " + this.numPods++; //TODO construct a pod continuing in a longer path
                for (int i = 0; i < 20; i++)
                {
                    podString += " " + tubeIds.X + " " + tubeIds.Y;
                }
                podString += ';';
                this.resources -= 1000;
            }
            res += tubeString + podString;
        }
        return res;
    }
}