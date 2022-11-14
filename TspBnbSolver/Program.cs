using TspUtils;

namespace TspBnbSolver
{
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
                return 1;
            
            foreach (ConfigurationLine configurationLine in configurationData.ConfigurationLines)
            {
                Console.WriteLine($"Solving {configurationLine.FileName}");
                
                MatrixData? matrixData = MatrixDataLoader
                    .DispatcherLoader($"Data/{configurationLine.FileName}");

                if (matrixData == null)
                    continue;

                Console.Write(matrixData.ToString());
                
                List<TspSolution> solutions = new List<TspSolution>();
                bool shouldSaveResult = true;
                
                for (int i = 0; i < configurationLine.AlgorithmPassCount; i++)
                {
                    try
                    {
                        var solution = new DfsBnbSolver(matrixData).Solve(0);
                        
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

                if (shouldSaveResult)
                {
                    var times = solutions.Select(solution => solution.ExecutionTime.TotalMilliseconds).ToList();
                    
                    TspSolutionToFileExporter.WriteToFullCv(
                        $"Solutions/{configurationLine.FileName.Replace(".txt", "").Replace(".tsp", "")}_result.csv",
                        configurationLine.FileName,
                        solutions[0],
                        times);

                    int average = (int) Math.Round(times.Average(), 0, MidpointRounding.AwayFromZero);

                    List<List<int>> result = new (1);
                    result.Add(new List<int>() {matrixData.NumberOfVertices, average});
                    
                    TspSolutionToFileExporter.WriteToScientificGraphCv("Solutions/solutions_data.csv", result);
                }

            }
            
            return 0;
        }
    }
}