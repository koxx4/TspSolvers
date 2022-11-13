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
                    Console.WriteLine("Błąd podczas rozwiazywania!: " + e.Message);
                }
            }
                
            TspSolutionToFileExporter.WriteToFullCv(
                $"Solutions/{configurationLine.FileName.Replace(".txt", "").Replace(".tsp", "")}_result.csv",
                configurationLine.FileName,
                solutions[0],
                solutions.Select(solution => solution.ExecutionTime.TotalMilliseconds).ToList());
        }

        return 0;
    }
}
