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
                
                TspSolution? oneSolution = null;
                
                for (int i = 0; i < configurationLine.AlgorithmPassCount; i++)
                {
                    try
                    {
                        oneSolution = BnbSolver.SolveUsingDfs(matrixData, 0);
                    //oneSolution = new DynamicSolver(matrixData).Solve(0);

                    Console.WriteLine($"E: {configurationLine.OptimalWeight} " +
                                      $"W: {oneSolution.MinPathWeight}, " +
                                      $"P: {string.Join("->", oneSolution.MinPath)}, " +
                                      $"T: {oneSolution.ExecutionTime.TotalMilliseconds} ms, " +
                                      $"M: {oneSolution.BytesUsed}B");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Błąd podczas rozwiazywania!!!\n" + e.Message);
                    }
                }
            }

            return 0;
        }
    }
}