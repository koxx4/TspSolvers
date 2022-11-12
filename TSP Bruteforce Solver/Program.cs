using System.Diagnostics;
using Kaos.Combinatorics;
using Pwilk.Tsp.Solvers.Bruteforce;
using TspUtils;

namespace TSP_Solvers;

internal static class Program
{
    private static void WriteResultToCv(string filename, string oldFile, TspSolution tspSolution, List<long> timeMeasurments)
    {
        var file = File.Create(filename);

        TextWriter tw = new StreamWriter(file);

        var avg = Math.Round(timeMeasurments.Average(), 2, MidpointRounding.AwayFromZero);

        tw.Write($"{oldFile} {avg} {tspSolution}\n");

        foreach (var timeMeasurment in timeMeasurments)
        {
            tw.WriteLine(timeMeasurment);
        }

        tw.Flush();
        tw.Close();
    }

    private static int Main(string[] args)
    {
        if (!File.Exists("settings.ini"))
        {
            Console.WriteLine("Nie mogłem znaleźć pliku inicjalizującego settings.ini!\nWciśnij jakiś przycisk by zakończyć");

            Console.ReadKey(true);

            return 1;
        }
        
        ConfigurationData? configurationData = ConfigurationData
            .LoadFromFile("settings.ini");

        if (configurationData == null)
        {
            return 1;
        }

        foreach (ConfigurationLine configurationLine in configurationData.ConfigurationLines)
        {
            MatrixData? matrixData = MatrixDataLoader
                .DispatcherLoader(configurationLine.FileName);

            if (matrixData == null)
            {
                continue;
            }

            Console.WriteLine($"Solving {configurationLine.FileName}");
            Console.WriteLine($"Optimal path should be {configurationLine.OptimalWeight}");

            Console.Write(matrixData.ToString());

            List<long> timeMeasurments = new(configurationLine.AlgorithmPassCount);
            TspSolution? oneSolution = null;

            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < configurationLine.AlgorithmPassCount; i++)
            {
                var matrix = matrixData.AdjacencyMatrixArray;

                stopwatch.Start();

                oneSolution = SolveTsp(matrix, 0);

                stopwatch.Stop();

                Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms, solution: {oneSolution}");

                timeMeasurments.Add(stopwatch.ElapsedMilliseconds);

                stopwatch.Reset();
            }

            WriteResultToCv($"{configurationLine.FileName.Replace(".txt", "")}_result.csv", configurationLine.FileName, oneSolution, timeMeasurments);
        }

        return 0;
    }
    
    private static TspSolution SolveTsp(int[,] graphMatrix, int startingVertex)
    {
        int matrixSize = graphMatrix.GetLength(1);
        List<int> verticesToVisit = new(matrixSize);

        //Generate list of vertices
        for (int i = 0; i < matrixSize; i++)
            if (i != startingVertex)
                verticesToVisit.Add(i);


        //Generate all permutations of vertices not including starting vertex
        Permutation permutation = new Permutation(verticesToVisit.Count);

        int minPathWeight = int.MaxValue;
        List<int> minPath = new (matrixSize);

        foreach (var permutationRow in permutation.GetRows())
        {
            List<int> permutationOfVertices = Permutation.Permute(permutationRow, verticesToVisit);
            int currentPathweight = 0;
            List<int> currentMinPath = new (matrixSize);

            //Path weight from starting point to next vertex
            int startToFirstInPermutationCost = graphMatrix[permutationOfVertices[0], startingVertex];
            currentPathweight += startToFirstInPermutationCost;
                
            currentMinPath.Add(startingVertex + 1);

            //Path weights from current permutation
            for (int i = 0; i < permutationOfVertices.Count - 1; i++)
            {
                int nextCost = graphMatrix[permutationOfVertices[i + 1], permutationOfVertices[i]];
                currentPathweight += nextCost;

                currentMinPath.Add(permutationOfVertices[i] + 1);
            }
            currentMinPath.Add(permutationOfVertices[^1] + 1);

            //Path weight from last vertex back to starting point
            int endToStartingPoint = graphMatrix[startingVertex, permutationOfVertices[^1]];
            currentPathweight += endToStartingPoint;

            currentMinPath.Add(startingVertex + 1);

            if (currentPathweight < minPathWeight)
            {
                minPathWeight = currentPathweight;
                minPath = currentMinPath;
            }
        }

        return new(minPathWeight, minPath);
    }
}