using System;
using System.Collections.Generic;

namespace VehicleRouting.Algorithms
{
    public abstract class Abstract
    {
        public Abstract(
            int seed,
            Action<int> writeAlgoIdentifier,
            Func<List<List<int>>, bool> areRoutesSolution,
            Func<List<List<int>>, int> evaluateRoutesTotalDistance,
            bool debugMode
        )
        {
            DebugMode = debugMode;
            Random = new Random(seed);
            WriteAlgoIdentifier = writeAlgoIdentifier;
            AreRoutesSolution = areRoutesSolution;
            EvaluateRoutesTotalDistance = evaluateRoutesTotalDistance;
        }
        public readonly bool DebugMode;
        public readonly Random Random;

        // invoke this every so often with an int that cannot be guessed without running your algo
        public readonly Action<int> WriteAlgoIdentifier;
        // helper func to check whether your routes are a solution
        public readonly Func<List<List<int>>, bool> AreRoutesSolution;
        // helper func to evaluate the distance travelled by your routes. returns -1 if your routes are invalid
        public readonly Func<List<List<int>>, int> EvaluateRoutesTotalDistance;
        public abstract List<List<int>> Solve(int[] demands, int[,] distanceMatrix, int vehicleCapacity, int maxDistance);
    }
    public class Debug
    {
        private static void _Log(object message, string level) =>
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff")} - [{level}] - {message}");
        public static void Log(object message) => _Log(message, "DEBUG");
        public static void LogError(object message) => _Log(message, "ERROR");
        public static void LogWarning(object message) => _Log(message, "WARNING");
        public static void LogException(object message) => _Log(message, "EXCEPTION");
    }
}
