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
            Building[] buildings = new Building[numNewBuildings + currentState.numBuildings];
            for (int i = 0; i < numNewBuildings; i++)
            {
                string buildingProperties = "2 3 95 38"; // type buildingId coordX coordY || 0 buildingId coordX coordY numAstronauts astronautType1 astronautType2 ...
                buildings[i] = ToBuilding(buildingProperties);
            }
            
            currentState.resources += resources;
            TravelRoute[] newTravelRoutes = new TravelRoute[currentState.travelRoutes.Length + travelRoutes.Length];
            travelRoutes.CopyTo(newTravelRoutes, 0);
            currentState.travelRoutes.CopyTo(newTravelRoutes, travelRoutes.Length);
            currentState.travelRoutes = newTravelRoutes;
            Pod[] newPods = new Pod[currentState.numPods + pods.Length];


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

public class GameState
{
    public int points, resources;
    public int numBuildings, numPods, numTravelRoutes;
    public GameDate date;
    public Building[] buildings;
    public Pod[] pods;
    public TravelRoute[] travelRoutes;
    public GameState(int numBuildings, int numPods, int numTravelRoutes)
    {
        points = 0;
        this.numBuildings = numBuildings;
        this.numPods = numPods;
        this.numTravelRoutes = numTravelRoutes;
        GameDate date;
        date.day = 0;
        date.month = 0;
        this.date = date;
        buildings = new Building[numBuildings];
        pods = new Pod[numPods];
        travelRoutes = new TravelRoute[numTravelRoutes];
    }
}