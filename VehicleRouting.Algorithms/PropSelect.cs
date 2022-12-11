/*
 * Extending Maverick's RandomSolverMoreBoosted to use fitness proportional selection, instead of top-N random choice.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace VehicleRouting.Algorithms
{
    public class PropSelect : Abstract
    {
        public PropSelect(
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
            int[,] savings = calculate_savings(demands, distanceMatrix);

            for (int attempt = 1; attempt <= 600; attempt++)
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
                            .OrderBy(pair => savings[currentNode,pair.Node]).Reverse()
                            .ToList();

                        if (eligibleNodes.Count() > 0)
                        {
                            if (eligibleNodes.Count() == 1)
                            {
                                var select = eligibleNodes[0];
                                capacity -= demands[select.Node];
                                route.Add(select.Node);
                                notVisited.RemoveAt(select.Index);
                                currentNode = select.Node;
                            }
                            else
                            {
                                List<int> weights = new List<int>();
                                int running_total = 0;
                                //Cumulative distance
                                foreach (var node in eligibleNodes)
                                {
                                    int score = running_total + distanceMatrix[currentNode, node.Node];
                                    weights.Add(score);
                                    running_total = score;

                                }
                                int current_total = 0;
                                //Want to maximise good solutions, scores are currently minimised. Take proportion.
                                for(int i=0; i<weights.Count; i++)
                                {
                                    weights[i] = running_total - weights[i];

                                }
                                var rand = Random.Next(weights[0]);
                                bool hasAdded = false;
                                for (int i = 1; i < weights.Count; i++)
                                {
                                    if (rand > weights[i])
                                    {
                                        var chosen = eligibleNodes[i];
                                        capacity -= demands[chosen.Node];
                                        route.Add(chosen.Node);
                                        notVisited.RemoveAt(chosen.Index);
                                        currentNode = chosen.Node;
                                        hasAdded = true;
                                        break;
                                    }
                                }
                                if (!hasAdded)
                                {
                                    var chosen = eligibleNodes[weights.Count - 1];
                                    capacity -= demands[chosen.Node];
                                    route.Add(chosen.Node);
                                    notVisited.RemoveAt(chosen.Index);
                                    currentNode = chosen.Node;
                                    hasAdded = true;
                                }
                            }


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
        private int[,] calculate_savings(int[] demands, int[,] distanceMatrix)
        {
            int[,] savings = new int[distanceMatrix.GetLength(0), distanceMatrix.GetLength(1)];

            for (int i = 0; i < distanceMatrix.GetLength(0) - 1; i++)
            {
                int zi = distanceMatrix[0, i];
                for (int j = i + 1; j < distanceMatrix.GetLength(0); j++)
                {
                    int zj = distanceMatrix[0, j];
                    int ij = distanceMatrix[i, j];
                    // node1, node2, savings, demands
                    int save = (zi + zj) - ij;
                    savings[i,j] = save;
                    savings[j, i] = save;

                }
            }
            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                savings[i, i] = -500;
            }
            return savings;
        }
    }
}
