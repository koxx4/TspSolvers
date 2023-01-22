using System.Diagnostics;
using TspAntColony.Configuration;
using TspUtils;

namespace TspAntColony.Algorithm;

public class AcTspSolver
{
    private readonly Random _random = new();
    
    // Parametry algorytmu
    private const double MinimumPheromoneValue = 0.01;
    private const double Rho = 0.5;
    private const int Iterations = 50;
    private const int StartingVertexNumber = 0;
    private readonly double _alpha;
    private readonly double _beta;
    private readonly double _pheromoneInitialValue;
    private readonly int _antsCount;
    private readonly MatrixData _matrixData;
    private readonly IPheromoneSpreadingStrategy _spreadingStrategy;

    public AcTspSolver(AcConfigurationDataLine configurationDataLine, MatrixData matrixData)
    {
        _matrixData = matrixData;
        _alpha = configurationDataLine.Alpha;
        _beta = configurationDataLine.Beta;
        _antsCount = matrixData.NumberOfVertices;
        _pheromoneInitialValue = CalculatePheromoneInitialValue();
        _spreadingStrategy = GetPheromoneSpreadingStrategy(configurationDataLine.PheromoneSpreadingStrategy);
    }

    private IPheromoneSpreadingStrategy GetPheromoneSpreadingStrategy(PheromoneSpreadingStrategy strategy)
    {
        switch (strategy)
        {
            case PheromoneSpreadingStrategy.Qas:
                return new QasSpreadingStrategy();
            case PheromoneSpreadingStrategy.Cas:
                return new CasSpreadingStrategy();
            case PheromoneSpreadingStrategy.Das:
                return new DasSpreadingStrategy();
            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
        }
    }

    private double CalculatePheromoneInitialValue()
    {
        long estimatedCost = EstimatePathWeightUsingGreedyAlgorithm();

        return _antsCount / (double) estimatedCost;
    }

    private long EstimatePathWeightUsingGreedyAlgorithm()
    {
        int[,] matrix = _matrixData.AdjacencyMatrixArray;
        int currentMinimumCost = int.MaxValue;
        int lastVertex = 0;
        List<int> minimumCosts = new();
        
        for (int i = 0; i < _matrixData.NumberOfVertices; i++)
        {
            for (int j = 0; j < _matrixData.NumberOfVertices; j++)
            {
                //Pomiń zapętlenie
                if (i == j) continue;

                // z i -> j
                int cost = matrix[j, i];
                
                if (cost < currentMinimumCost)
                {
                    currentMinimumCost = cost;
                    lastVertex = j;
                }
            }
            minimumCosts.Add(currentMinimumCost);
        }

        //Dodaj koszt ścieżki powrotnej
        minimumCosts.Add(matrix[0, lastVertex]);

        return minimumCosts.Sum();
    }

    public TspSolution Solve()
    {
        Stopwatch stopwatch = new();
        
        stopwatch.Start();
        List<int> bestPath = DoSolve();
        stopwatch.Stop();
        
        bestPath.Add(StartingVertexNumber);

        int cost = GetPathLength(bestPath, _matrixData.AdjacencyMatrixArray);
        
        return new TspSolution(cost, bestPath, stopwatch.Elapsed);
    }

    private List<int> DoSolve()
    {
        int[,] matrix = _matrixData.AdjacencyMatrixArray;
        double[,] pheromoneMatrix = CreateAndInitializePheromoneMatrix();
        List<List<int>> pathsInEachIteration = new();

        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            //Kryterium stopu - wszystkie mrówki odwiedziły każde możliwe miasto
            bool allAntsTraversedAllMatrix = false;
            
            List<List<int>> antsPositions = new(_antsCount);

            for (int i = 0; i < _antsCount; i++)
            {
                antsPositions.Add(new List<int>(_matrixData.NumberOfVertices) { StartingVertexNumber });
            }

            // Główna pętla algorytmu
            while(!allAntsTraversedAllMatrix)
            {
                //Lista ... przejszniętych(?) krawędzi w tym obiegu
                // aby było wiadomo, na których krawędziach zaktualizować feromon
                // droga: [1] -> [0]
                List <int[]> traversedEdges = new();
                
                //Dla każdej mrówki
                for (int ant = 0; ant < _antsCount; ant++)
                {
                    List<int> antPath = antsPositions[ant];
                    
                    //Ostatnie miasto na liście - obecny wierzchołek, na którym jest mrufka
                    int antCurrentVertex = antPath[^1];

                    //Oblicz mianownik, który będzie normalizował wartości prawdopodobieństw 
                    //do zakresu (0,1)
                    double denominator = GetCurrentProbDenominator(antPath, matrix, antCurrentVertex, pheromoneMatrix);

                    //Rzeczywiscie losując prawdopodbieństwo
                    //Rozpatruj każdą możliwą krawędź z obecnego wierzchołka, na którym znajduje się mrówka
                    int nextVertexThatAntShouldGoTo = antCurrentVertex;
                    double accumulatedProbability = 0.0;
                    double probability = _random.NextDouble();
                    for (int nextPossibleVertex = 0; nextPossibleVertex < _matrixData.NumberOfVertices; nextPossibleVertex++)
                    {
                        //Sprawdzenie czy mrufka nie była już przypadkiem w wylosowanym kolejnym wierzchołku
                        if (antPath.Contains(nextPossibleVertex)) continue;

                        int pathCost = matrix[nextPossibleVertex, antCurrentVertex];
                        double pheromoneValueAtThisPath = pheromoneMatrix[nextPossibleVertex, antCurrentVertex];

                        //Jeśli koszt jest ujemny lub 0 oznacza brak przejścia
                        if (pathCost <= 0) continue;

                        //btw gdyby tu było 0 to byłby błąd dzielenia ;)
                        double normalizedCost = 1.0 / pathCost;

                        double probabilityOfAntMove = 
                           (Math.Pow(pheromoneValueAtThisPath, _alpha ) * Math.Pow(normalizedCost, _beta)) / denominator;

                        accumulatedProbability += probabilityOfAntMove;

                        accumulatedProbability = accumulatedProbability >= 0.99 ? 1.0 : accumulatedProbability;

                        if (probability <= accumulatedProbability)
                        {
                            nextVertexThatAntShouldGoTo = nextPossibleVertex;
                            break;
                        }
                    }
                    
                    //Teraz następuje rusznięcie mrufkom na najbardziej obiecujący wierzchołek
                    antsPositions[ant].Add(nextVertexThatAntShouldGoTo);
                    traversedEdges.Add(new []{nextVertexThatAntShouldGoTo, antCurrentVertex});
                }
                
                //Sprawdzenie czy mrówki już przeszły wszystkie wierzchołki
                allAntsTraversedAllMatrix = antsPositions
                    .Select(antPositions => antPositions.Count)
                    .All(traversedVerticesCount => traversedVerticesCount >= _matrixData.NumberOfVertices);
                
                //Aktualizowanie feromonu za pomocą ustalonej wcześniej strategii
                foreach (var edge in traversedEdges)
                {
                    int from = edge[1];
                    int to = edge[0];
                    int edgeCost = matrix[to, from];
                    
                    pheromoneMatrix[to, from] += _spreadingStrategy.GetSpreadValue(edgeCost, 0);
                }
                
                //Aktualizowanie macierzy feromonu - zanikanie feromonu na ścieżkach
                //przy użyciu parametru Rho
                EvaporatePheromoneMatrix(ref pheromoneMatrix);
            }

            pathsInEachIteration.Add(GetBestPath(antsPositions, matrix));
        }

        return GetBestPath(pathsInEachIteration, matrix);
    }

    private double GetCurrentProbDenominator(List<int> antPath, int[,] matrix, int antCurrentVertex, double[,] pheromoneMatrix)
    {
        double denominator = 0.0;
        for (int nextPossibleVertex = 0; nextPossibleVertex < _matrixData.NumberOfVertices; nextPossibleVertex++)
        {
            //Sprawdzenie czy mrufka nie była już przypadkiem w wylosowanym kolejnym wierzchołku
            if (antPath.Contains(nextPossibleVertex)) continue;

            int pathCost = matrix[nextPossibleVertex, antCurrentVertex];
            double pheromoneValueAtThisPath = pheromoneMatrix[nextPossibleVertex, antCurrentVertex];

            //Jeśli koszt jest ujemny lub 0 oznacza brak przejścia
            if (pathCost <= 0) continue;

            //btw gdyby tu było 0 to byłby błąd dzielenia ;)
            double normalizedCost = 1.0 / pathCost;

            double probabilityOfAntMove =
                Math.Pow(pheromoneValueAtThisPath, _alpha) * Math.Pow(normalizedCost, _beta);

            denominator += probabilityOfAntMove;
        }

        return denominator;
    }

    private static void EvaporatePheromoneMatrix(ref double[,] pheromoneMatrix)
    {
        for (var i = 0; i < pheromoneMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < pheromoneMatrix.GetLength(1); j++)
            {
                pheromoneMatrix[i, j] *= Rho;

                if (pheromoneMatrix[i, j] <= MinimumPheromoneValue)
                {
                    pheromoneMatrix[i, j] = MinimumPheromoneValue;
                }
            }
        }
    }
    
    // Inicjalizacja początkowych pól feromonu
    private double[,] CreateAndInitializePheromoneMatrix()
    {
        int n = _matrixData.NumberOfVertices;
        double[,] pheromoneMatrix = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                pheromoneMatrix[i, j] = _pheromoneInitialValue;
            }
        }

        return pheromoneMatrix;
    }
    
    private List<int> GetBestPath(List<List<int>> paths, int[,] distances)
    {
        int bestIndex = 0;
        int bestLength = GetPathLength(paths[0], distances);
        for (int i = 1; i < paths.Count; i++)
        {
            int length = GetPathLength(paths[i], distances);
            if (length < bestLength)
            {
                bestIndex = i;
                bestLength = length;
            }
        }
        
        return paths[bestIndex];
    }

    private int GetPathLength(List<int> path, int[,] distances)
    {
        int length = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            length += distances[path[i], path[i + 1]];
        }
        return length;
    }
}