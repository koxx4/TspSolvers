using System.Diagnostics;
using System.Runtime.InteropServices;
using TspUtils;

namespace TspDynamicSolver;

public class DynamicSolver
{
    private readonly int[,] _matrix;
    private readonly int _numberOfVertices;
    private readonly ulong[,] _memo;

    public DynamicSolver(MatrixData matrixData)
    {
        _numberOfVertices = matrixData.NumberOfVertices;
        _matrix = matrixData.AdjacencyMatrixArray;
        _memo = new ulong[ _numberOfVertices, 1 << _numberOfVertices];

        for (int i = 0; i < _memo.GetLength(0); i++)
        {
            for (int j = 0; j < _memo.GetLength(1); j++)
            {
                _memo[i, j] = ulong.MaxValue;
            }
        }
    }

    public TspSolution Solve(int startingVertex)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Setup(startingVertex);
        DoSolve(startingVertex);

        int minCost = FindMinCost(startingVertex);
        int[] minPath = RecreateMinPath(startingVertex);

        stopwatch.Stop();

        long bytesUsed = sizeof(int) * _memo.GetLength(0) * _memo.GetLength(1)
                         + Buffer.ByteLength(_matrix)
                         + sizeof(int);

        return new(minCost, minPath.ToList(), stopwatch.Elapsed, bytesUsed);
    }

    private void Setup(int startingVertex)
    {
        for (int i = 0; i < _numberOfVertices; i++)
        {
            if (i == startingVertex)
                continue;

            _memo[i, 1 << startingVertex | 1 << i] = (ulong) _matrix[i, startingVertex];
        }
    }

    private void DoSolve(int startingNode)
    {
        for (int i = 3; i <= _numberOfVertices; i++)
        {
            foreach (var combination in BitsCombinations(i, _numberOfVertices))
            {

                if (IsVertexNotInSubset((ulong) startingNode, combination))
                    continue;

                for (int nextNode = 0; nextNode < _numberOfVertices; nextNode++)
                {

                    if (nextNode == startingNode || IsVertexNotInSubset((ulong) nextNode, combination))
                        continue;

                    ulong state = combination ^ (1ul << nextNode);
                    int minDistance = int.MaxValue;

                    for (long endNode = 0; endNode < _numberOfVertices; endNode++)
                    {
                        
                        if (endNode == startingNode || endNode == nextNode || IsVertexNotInSubset((ulong) endNode, combination))
                            continue;

                        int newDistance = (int) _memo[(ulong) endNode, state] + _matrix[endNode, nextNode];

                        if (newDistance < minDistance)
                            minDistance = newDistance;
                    }
                    _memo[nextNode, combination] = (ulong) minDistance;
                }
                
            }
            
        }
    }

    private int FindMinCost(int startingNode)
    {
        int finalState = (1 << _numberOfVertices) - 1;
        int minTourCost = int.MaxValue;

        for (int i = 0; i < _numberOfVertices; i++)
        {
            if (i == startingNode)
                continue;

            int tourCost = (int) _memo[i, finalState] + _matrix[startingNode, i];

            if (tourCost < minTourCost)
            {
                minTourCost = tourCost;
            }
        }

        return minTourCost;
    }

    private int[] RecreateMinPath(int startingNode)
    {
        int lastIndex = startingNode;
        int state = (1 << _numberOfVertices) - 1;
        int[] tour = new int[_numberOfVertices + 1];

        for (int i = _numberOfVertices - 1; i >= 1; i--)
        {
            
            int index = -1;
            for (int j = 0; j < _numberOfVertices; j++)
            {
                if (j == startingNode || IsVertexNotInSubset((ulong) j ,(ulong) state))
                    continue;

                if (index == -1)
                    index = j;

                int previousDistance = (int) _memo[index, state] + _matrix[index, lastIndex];
                int newDistance = (int) _memo[j, state] + _matrix[j, lastIndex];

                if (newDistance < previousDistance)
                    index = j;
            }

            tour[i] = index;
            state ^= 1 << index;
            lastIndex = index;
        }

        tour[0] = tour[_numberOfVertices] = startingNode;
        return tour;
    }

    private bool IsVertexNotInSubset(ulong vertex, ulong subset)
    {
        return ((1ul << (int) vertex) & subset) == 0;
    }

    private IEnumerable<ulong> BitsCombinations(int bitsSetToOneCount, int wordLength)
    {
        List<ulong> subsets = new(wordLength);

        foreach (var combination in BitsCombinations(0, 0, bitsSetToOneCount, wordLength, subsets))
        {
            yield return combination;
        }
    }

    private IEnumerable<ulong> BitsCombinations(ulong set, int at, int bitsSetToOneCount, int wordLength, List<ulong> subsets)
    {
        if (bitsSetToOneCount == 0)
        {
            yield return set;
        }
        else
        {
            for (int i = at; i < wordLength; i++)
            {
                set |= (ulong) (1 << i);

                foreach (var combination in BitsCombinations(set, i + 1, bitsSetToOneCount - 1, wordLength, subsets))
                {
                    yield return combination;
                }
                
                set &= (ulong)  ~(1 << i);
            }
        }
    }
}