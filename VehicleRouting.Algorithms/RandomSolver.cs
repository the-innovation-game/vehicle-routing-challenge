using System;
using System.Collections.Generic;
using System.Linq;

namespace VehicleRouting.Algorithms
{
    public class RandomSolver : Abstract
    {
        public RandomSolver(
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
            for (int attempt = 1; attempt <= 1000; attempt++)
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

                // generate and write an int that cannot be guessed without running your algorithm
                WriteAlgoIdentifier(Random.Next() / routes.Count * routes[0].Sum());
                if (AreRoutesSolution(routes))
                    return routes;
            }
            return new();
        }
    }
}
