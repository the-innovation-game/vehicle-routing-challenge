/*
    Acknowledgements:
    Increased number of tries in RandomSolver (this should generate more tokens on "powerful" computers where the bottleneck is the rate limit, not CPU power)
*/

using System;
using System.Collections.Generic;

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
       public override List<bool> Solve(int numVariables, List<List<int>> clauses)
        {
            for (int attempt = 1; attempt <= 2000; attempt++)
            {
                if (DebugMode && attempt % 200 == 0)
                    Debug.Log($"Number of Attempts: {attempt}");

                List<bool> input = new List<bool>();
                int numVariablesRemaining = numVariables;
                while (numVariablesRemaining != 0)
                {
                    int n = Math.Min(numVariablesRemaining, 30);
                    int nBits = Random.Next(1 << n);
                    for (int i = 0; i < n; i++)
                        input.Add((nBits & (1 << i)) != 0);
                    numVariablesRemaining -= n;
                }

                // generate and write a unique integer that identifies when someone is using your algorithm
                int uniqueInt = 1;
                for (int i = 1; i < input.Count; i++)
                    uniqueInt *= input[i] ? i : 1;
                WriteAlgoIdentifier(uniqueInt);

                // check if solution has been found
                if (IsInputSolution(input))
                    return input;
            }
            return new();
        }
    }
}
