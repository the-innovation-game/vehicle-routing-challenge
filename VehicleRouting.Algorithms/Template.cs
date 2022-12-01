using System;
using System.Collections.Generic;

namespace VehicleRouting.Algorithms
{
    public class Template : Abstract
    {
        public Template(
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
            if (DebugMode)
                Debug.Log("Hello world!");

            return new();
        }
    }
}
