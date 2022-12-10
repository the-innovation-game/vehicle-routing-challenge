/*
    Acknowledgements:
    Increased number of tries in RandomSolver (this should generate more tokens on "powerful" computers where the bottleneck is the rate limit, not CPU power)
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace VehicleRouting.Algorithms
{
    public class BetterRandomSolver : Abstract
    {
        public BetterRandomSolver(
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
            for (int attempt = 1; attempt <= 10000; attempt++)
            {
                if (DebugMode && attempt % 200 == 0)
                    Debug.Log($"Number of Attempts: {attempt}");

                List<List<int>> routes = new();
                List<int> notVisited = Enumerable.Range(1, demands.Length - 1).ToList();
                while (notVisited.Count > 0)
                {
                    List<int> route = new();

                    int currentNode = 0;
                    int capacity = vehicleCapacity;

                    while (capacity > 0 && notVisited.Count > 0)
                    {
                        var eligibleNodes = notVisited
                            .Select((node, i) => new { Index = i, Node = node })
                            .Where(pair => demands[pair.Node] <= capacity)
                            .OrderBy(pair => distanceMatrix[currentNode, pair.Node])
                            .ToList();

                        if (eligibleNodes.Count() > 0)
                        {
                            var chosen = eligibleNodes[Random.Next((int)Math.Ceiling(eligibleNodes.Count / 2.0))];
                            capacity -= demands[chosen.Node];
                            route.Add(chosen.Node);
                            notVisited.RemoveAt(chosen.Index);
                            currentNode = chosen.Node;
                        }
                        else
                            break;
                    }

                    routes.Add(route);
                }

                // generate and write a unique integer that identifies when someone is using your algorithm
                int uniqueInt = 1;
                for (int i = 0; i < routes.Count; i++)
                    uniqueInt *= routes[i].Sum();
                WriteAlgoIdentifier(uniqueInt);

                // check if solution has been found
                if (AreRoutesSolution(routes))
                    return routes;
            }
            return new();
        }
    }
}
