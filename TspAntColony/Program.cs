using TspAntColony.Algorithm;
using TspAntColony.Configuration;
using TspSimulatedAnnealingSolver.Algorithm;
using TspSimulatedAnnealingSolver.Configuration;
using TspUtils;

namespace TspAntColony;

public static class Program
{

    public static void Main(string[] args)
    {
        AcFileConfigurationDataLoader configurationDataLoader = new("./Data/settings-test.csv");
        AcConfigurationData? configurationData = configurationDataLoader.LoadConfiguration();

        Directory.CreateDirectory("Solutions");

        foreach (var configurationLine in configurationData.ConfigurationLines)
        {
            MatrixData? matrixData = MatrixDataLoader.DispatcherLoader($"Data/{configurationLine.FileName}");

            if (matrixData == null)
            {
                Console.WriteLine("Skipping! No data could be read!");
                continue;
            }

            Console.WriteLine($"Solving {configurationLine.FileName}");

            List<TspSolution> solutions = new List<TspSolution>();
            List<double> errors = new List<double>();
            bool shouldSaveResult = true;

            Console.WriteLine("-----------------------------------");
            for (int i = 0; i < configurationLine.AlgorithmPassCount; i++)
            {
                try
                {
                    TspSolution solution = new AcTspSolver(configurationLine, matrixData).Solve();

                    solutions.Add(solution);

                    double errorMargin = solution.MinPathWeight / (double) configurationLine.OptimalWeight;
                    errorMargin -= 1;
                    errorMargin *= 100;
                    errorMargin = Math.Round(errorMargin, 3, MidpointRounding.ToEven);

                    errors.Add(errorMargin);

                    string timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                    Console.WriteLine($"({i + 1}/{configurationLine.AlgorithmPassCount})|{timestamp}|" +
                                      $"E: {configurationLine.OptimalWeight} " +
                                      $"W: {solution.MinPathWeight}, " +
                                      //$"P: {string.Join("->", solution.MinPath)}, " +
                                      $"T: {solution.ExecutionTime.TotalMilliseconds} ms, " +
                                      $"ERR: {errorMargin}%");
                }
                catch (AlgorithmTimeoutException timeoutException)
                {
                    Console.WriteLine($"ABORTING {configurationLine.FileName}");
                    Console.WriteLine(timeoutException.Message);

                    string fileName = AcConfigurationBasedFilenameBuilder
                        .BuildName(configurationLine, matrixData, "err");

                    File.WriteAllText($"Solutions/{fileName}",
                        $"ABORTING {configurationLine.FileName}\n{timeoutException.Message}");

                    shouldSaveResult = false;

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Błąd podczas rozwiazywania!: " + e.StackTrace);
                    shouldSaveResult = false;
                }
            }
            
                    
            if (solutions.Count > 0 && errors.Count > 0 && shouldSaveResult)
            {
                var times = solutions.Select(solution => solution.ExecutionTime.TotalMilliseconds).ToList();
            
                int timeAverage = (int) Math.Round(times.Average(), 0, MidpointRounding.AwayFromZero);
                double errorAverage = Math.Round(errors.Average(), 3, MidpointRounding.AwayFromZero);
                
                Console.WriteLine($"AVG ERR: {errorAverage}%, AVG TIME: {timeAverage}[ms]");
                Console.WriteLine("Saving results");
                
                string fileName = AcConfigurationBasedFilenameBuilder
                    .BuildName(configurationLine, matrixData, "csv");
                
                
                TspSolutionToFileExporter.WriteToFullCvWithErrors(
                    $"Solutions/result_{fileName}",
                    configurationLine.FileName,
                    solutions,
                    errors);

                ScientificCsvDataLineWithErrorRate dataLine = new ScientificCsvDataLineWithErrorRate
                {
                    VerticesCount = matrixData.NumberOfVertices,
                    Time = timeAverage,
                    ErrorRate = errorAverage
                };

                string scientificFileName = AcConfigurationBasedFilenameBuilder
                    .BuildName(configurationLine, matrixData, "csv", false);
            
                TspSolutionToFileExporter.WriteToScientificGraphCvWithErrorRate($"Solutions/Aggregated/{scientificFileName}", dataLine);
            }
        }
    }
}