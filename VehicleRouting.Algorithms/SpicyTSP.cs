using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;

namespace VehicleRouting.Algorithms
{
    public class SpicyTSP : Abstract
    {
        public SpicyTSP(
            int seed,
            Action<int> writeAlgoIdentifier,
            Func<List<List<int>>, bool> areRoutesSolution,
            Func<List<List<int>>, int> evaluateRoutesTotalDistance,
            bool debugMode
        ) : base(seed, writeAlgoIdentifier, areRoutesSolution, evaluateRoutesTotalDistance, debugMode)
        {
        }
        public override List<List<int>> Solve(int[] demands, int[,] distanceMatrix, int vehicleCapacity, int maxDistance)
        {
            /*
                Your Objective:
                     Return a list of routes such that
                         1. every non-depot node is visited once only (node 0 is depot)
                         2. the sum of Demands on each route must not exceed the VehicleCapacity
                         3. total distance of your routes is less than MaxDistance

                Rules:
                Rules:
                     1. You should implement this function
                     2. You are not allowed to use external libraries
                     3. You should invoke WriteAlgoIdentifier every so often with a dynamically generated integer
                        * This allows us to verify when someone is using your algorithm
                        e.g. multiply all non-zero index of True in your current solution attempt
                     4. If you need to generate random numbers, you must use this.Random
                     5. If you want to give up on the challenge (e.g. maybe its unsolvable), you should return new()
                     6. Your algorithm name must be less than or equal to 20 characters (alpha-numeric only)
                     7. Your class name and filename must be `<algorithm_name>.cs` 
                     8. All your utility classes should be nested in this class or contained in a namespace unique to your algorithm
                     9. If you are improving an existing algorithm, make a copy of the code before making modifications

                Example Challenge:
                     Demands:           { 0, 45, 55, 12 }
                     DistanceMatrix:
                         {
                             {   0, 175, 217, 257 },
                             { 175,   0,  96, 424 },
                             {  217, 96,   0, 216 },
                             { 257, 424, 216,   0 }
                         }
                     VehicleCapacity:   100
                     MaxDistance:       1161

                Example Solution (there may be more than one solution):
                     Routes: (each route implicitly starts and ends at depot)
                         {
                             { 1, 2 },
                             { 3 }
                         }

                Example Solution evaluated against Challenge:
                    Route 1:            0 -> 1 -> 2 -> 0
                    Route Demand:       45 + 55 = 100
                    Route Distance:     175 + 96 + 216 = 487

                    Route 2:            0 -> 3 -> 0
                    Route Demand:       12
                    Route Distance:     257 + 257 = 514

                    Total Distance = 487 + 514 = 1001

                    1001 is shorter than MaxTotalDistance
             */

            HashSet<int> unvisited = new HashSet<int>();
            for(int i = 1; i<demands.Length; i++)
            {
                unvisited.Add(i);
            }


            List<List<int>> savings = calculate_savings(demands, distanceMatrix);
            savings.Sort((x, y) => x[2].CompareTo(y[2]));
            savings.Reverse();

            List<List<int>> routes = new List<List<int>>();
            int counter = 0;
            int remainingDistance = maxDistance;
            while (unvisited.Count > 0 && remainingDistance>0)
            {
                if (counter > 100000000)
                {
                    return new();
                }
                int currentCapacity = vehicleCapacity;
                List<int> subroute = new List<int>();
                List<int> to_remove = new List<int>();
                int previous_location = 0;
                for (int i = 1; i < savings.Count; i++)
                {
                    {
                        if (unvisited.Count ==1)
                        {
                            foreach(int loc in unvisited)
                            {
                                if ((demands[loc] <= currentCapacity) && (distanceMatrix[previous_location, loc] <= remainingDistance))
                                {
                                    subroute.Add(i);
                                    unvisited.Remove(loc);
                                }
                            }
                        }
                        if (currentCapacity<=0 || remainingDistance <=0)
                        {
                            break;
                        }
                        
                        int dist = distanceMatrix[savings[i][0], savings[i][1]];
                        if ((savings[i][3] <= currentCapacity) && (((dist + distanceMatrix[previous_location, savings[i][0]]) <=  remainingDistance) || ((previous_location==0) && (dist<=remainingDistance))))
                        {
                            if (unvisited.Contains(savings[i][0]) && unvisited.Contains(savings[i][1]))
                            {
                                subroute.Add(savings[i][0]);
                                subroute.Add(savings[i][1]);
                                previous_location = savings[i][1];
                                unvisited.Remove(savings[i][0]);
                                unvisited.Remove(savings[i][1]);
                                to_remove.Add(i);
                                currentCapacity -= savings[i][3];
                                int tmp_dist = remainingDistance;
                                remainingDistance -= (dist + distanceMatrix[previous_location, savings[i][0]]);
                                int dist_delta = tmp_dist - remainingDistance;
                            }
                        }
                    }
                }
                if (subroute.Count > 0)
                {
                    routes.Add(subroute);
                }
                to_remove.Sort();
                to_remove.Reverse();
                foreach (int r in to_remove) savings.RemoveAt(r);
                counter++;
            }
            if(unvisited.Count > 0) {
                return new();
            }
            return routes;

        }

        private List<List<int>> calculate_savings(int[] demands, int[,] distanceMatrix)
        {
            List<List<int>> savings = new List<List<int>>();

            for(int i =1; i< distanceMatrix.GetLength(0)-1; i++)
            {
                int zi = distanceMatrix[0,i];
                for (int j = i+1; j < distanceMatrix.GetLength(0); j++)
                {
                    int zj = distanceMatrix[0, j];
                    int ij = distanceMatrix[i, j];
                    // node1, node2, savings, demands
                    List<int> list = new List<int> {i, j, (zi + zj) - ij, demands[i] + demands[j] };                    
                    savings.Add(list);

                }
            }
            return savings;
        }
    }
}
