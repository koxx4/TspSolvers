using TspUtils;

namespace TspDynamicSolver;

internal static class Program
{
    public static int Main(string[] args)
    {
        
        if (!File.Exists("Data/settings.ini"))
        {
            Console.WriteLine("Nie mogłem znaleźć pliku inicjalizującego settings.ini!\nWciśnij jakiś przycisk by zakończyć");

            Console.ReadKey(true);

            return 1;
        }
    
        ConfigurationData? configurationData = ConfigurationData
            .LoadFromFile("Data/settings.ini");

        if (configurationData == null)
        {
            return 1;
        }

        foreach (ConfigurationLine configurationLine in configurationData.ConfigurationLines)
        {
            MatrixData? matrixData = MatrixDataLoader
                .DispatcherLoader($"Data/{configurationLine.FileName}");

            if (matrixData == null)
            {
                continue;
            }

            Console.WriteLine($"Solving {configurationLine.FileName}");
            Console.Write(matrixData.ToString());
            
            List<TspSolution> solutions = new List<TspSolution>();
            bool shouldSaveResult = true;
                
            for (int i = 0; i < configurationLine.AlgorithmPassCount; i++)
            {
                try
                {
                    var solution = new DynamicSolver(matrixData).Solve(startingVertex: 0);
                        
                    solutions.Add(solution);

                    Console.WriteLine($"E: {configurationLine.OptimalWeight} " +
                                      $"W: {solution.MinPathWeight}, " +
                                      $"P: {string.Join("->", solution.MinPath)}, " +
                                      $"T: {solution.ExecutionTime.TotalMilliseconds} ms, " +
                                      $"M: {solution.BytesUsed}B");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Błąd podczas rozwiazywania!: " + e.StackTrace);
                    shouldSaveResult = false;
                }
            }

            if (shouldSaveResult)
            {
                var times = solutions.Select(solution => solution.ExecutionTime.TotalMilliseconds).ToList();
                var bytesUsed = solutions.Select(solution => solution.BytesUsed).First();
                
                int average = (int) Math.Round(times.Average(), 0, MidpointRounding.AwayFromZero);
                
                TspSolutionToFileExporter.WriteToFullCv(
                    $"Solutions/{configurationLine.FileName.Replace(".txt", "").Replace(".tsp", "")}_result.csv",
                    configurationLine.FileName,
                    solutions.First(),
                    times.ToList(),
                    bytesUsed);
                
                List<List<long>> result = new (1);
                result.Add(new List<long>() {matrixData.NumberOfVertices, average, bytesUsed});

                TspSolutionToFileExporter.WriteToScientificGraphWithMemoryCv("Solutions/solutions_data.csv", result);
            }

        }

        return 0;
    }
}
