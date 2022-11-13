using System.Diagnostics;
using System.Runtime.InteropServices;
using TspUtils;

namespace TspBnbSolver;

public class DfsBnbSolver
{
    private readonly int _verticesCount;
    private readonly int[,] _matrix;
    private readonly int[] _accumulatedWeightsCosts;
    private readonly int[] _bestKnownPath;
    private readonly Stopwatch _stopwatch = new();

    private int _bestKnownCost;
    
    public DfsBnbSolver(MatrixData matrixData)
    {
        _verticesCount = matrixData.NumberOfVertices;
        _matrix = matrixData.AdjacencyMatrixArray;
        _bestKnownCost = int.MaxValue;
        _bestKnownPath = new int[_verticesCount + 1];
        

        //Wygeneruj posortowane krawedzie wedlug kosztow od min do max
        var sortedWeights = matrixData.GetSortedWeights().ToArray();

        //Policz zsumowane koszty z posortowanych wag
        _accumulatedWeightsCosts = new int[_verticesCount];
        _accumulatedWeightsCosts[0] = sortedWeights[0];

        for (int i = 1; i < _accumulatedWeightsCosts.Length; i++)
            _accumulatedWeightsCosts[i] = _accumulatedWeightsCosts[i - 1] + sortedWeights[i];
    }
    
    public TspSolution Solve(int startingVertex)
    {
        _bestKnownPath[^1] = startingVertex;
        
        int[] currentPath = new int[_verticesCount];
        currentPath[0] = startingVertex;

        _stopwatch.Reset();
        _stopwatch.Start();
        GoNextTreeLevel(currentCost: 0, currentLevel: 1, currentPath, new HashSet<int>(new []{0} ));
        _stopwatch.Stop();

        return new TspSolution(_bestKnownCost, _bestKnownPath.ToList(), _stopwatch.Elapsed, 
            Buffer.ByteLength(_matrix) 
            + Buffer.ByteLength(_accumulatedWeightsCosts)
            + Buffer.ByteLength(_bestKnownPath)
            + Buffer.ByteLength(currentPath) );
    }

    private void GoNextTreeLevel(int currentCost, int currentLevel, int[] currentPath, HashSet<int> visitedVertices)
    {
        //Ostatni poziom drzewa
        if (currentLevel ==  _verticesCount)
        {
            //Dodaj do całkowitego kosztu koszt drogi powrotnej
            currentCost += _matrix[currentPath[0], currentPath[^1]];

            //Zapisz najlepsze znane obecnie wyniki
            if (currentCost < _bestKnownCost)
            {
                _bestKnownCost = currentCost;
                currentPath.CopyTo(_bestKnownPath, 0);
                _bestKnownPath[^1] = currentPath[0];
            }
            
            //Koniec rekursji --> go up
            return;
        }

        //Przechodzimy przez wszystkie mozliwe do wystapienia wezly
        for (int nextVertex = 0; nextVertex < _verticesCount; nextVertex++)
        {
            int jumpCost = _matrix[nextVertex, currentPath[currentLevel - 1]];
            
            int maxBound = _accumulatedWeightsCosts[_verticesCount - 1 - visitedVertices.Count];
            //Przechodzimy na kolejny węzeł jesli nie jest samym soba albo juz w nim nie bylismy,
            //albo wiemy ze bedzie jeszcze krotszy od najkrotszego znanego
            if (IsInvalidEdge(jumpCost) || visitedVertices.Contains(nextVertex) || currentCost + jumpCost + maxBound >= _bestKnownCost)
                continue;

            currentPath[currentLevel] = nextVertex;
            visitedVertices.Add(nextVertex);
            
            GoNextTreeLevel(currentCost + jumpCost, currentLevel + 1, currentPath, visitedVertices);
            
            //Wyczysz zbior odwiedzonych wierzholkow
            visitedVertices.Clear();
            
            //Rekonstrukcja odwiedzonych wierzcholkow
            for (int i = 0; i < currentLevel; i++)
            {
                visitedVertices.Add(currentPath[i]);
            }
        }
    }

    private bool IsInvalidEdge(int weight)
    {
        return weight <= 0;
    }
}