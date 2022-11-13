using System.Diagnostics;
using TspUtils;

namespace TspBnbSolver;

public static class BnbSolver
{
    public static TspSolution SolveUsingDfs(MatrixData matrixData, int startingVertex)
    {
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        var result = DoSolveUsingDfs(matrixData, startingVertex);
        stopwatch.Stop();

        return new TspSolution(result.weight, result.bestPath, stopwatch.Elapsed, result.memoryUsed);
    }
    
    private static (int weight, List<int> bestPath, long memoryUsed) DoSolveUsingDfs(MatrixData matrixData, int startingVertex)
    {
        int numberOfVertices = matrixData.NumberOfVertices;
        int[,] adjacencyMatrix = matrixData.AdjacencyMatrixArray;

        //Wygeneruj listę wierzchołków, bez wierzchołka początkowego
        int[] verticesToVisit = Enumerable.Range(0, numberOfVertices)
            .Where(x => x != startingVertex)
            .ToArray();

        //Wygeneruj posortowane krawedzie wedlug kosztow od min do max
        var sortedWeights = matrixData.GetSortedWeights().ToArray();

        //Policz zsumowane koszty z posortowanych wag
        var accumulatedCosts = new int[numberOfVertices - 1];
        accumulatedCosts[0] = sortedWeights[0];
        
        for (int i = 1; i < accumulatedCosts.Length; i++)
        {
            accumulatedCosts[i] = accumulatedCosts[i - 1] + sortedWeights[i];
        }

        //Przygotuj obecnie najlepsza sciezke i jej koszt
        int bestMinimalPathWeight = int.MaxValue;
        int[] bestPath = new int[verticesToVisit.Length];
        
        //Iteruj przez kazda mozliwa permutacje
        do
        {
            //Koszt obecnie badanej ścieżki
            int currentPathWeight = 0;
            //Czy obecna ścieżka jest lepsza od najlepszej znanej
            bool currentPathIsBetter = true;

            //Podazaj ta sciezka i licz jej koszt
            //Sprawdzaj na biezaco czy nie jest juz ona gorsza od obecnie najlepszej znanej
            for (int i = 0; i < verticesToVisit.Length - 1; i++)
            {
                //Na tym wierzcholku jestesmy
                int currentNode = verticesToVisit[i];
                int nextNode = verticesToVisit[i + 1];

                //Jesli poczatek sciezki to dodaj koszt z poczatkowego node'a do obecnego
                if (i == 0)
                {
                    currentPathWeight += adjacencyMatrix[currentNode, startingVertex];
                }
                
                //Jestesmy na przedostatnim, dodaj powrotna sciezke
                if (i == verticesToVisit.Length - 2)
                {
                    currentPathWeight += adjacencyMatrix[startingVertex, nextNode];
                }

                currentPathWeight += adjacencyMatrix[nextNode, currentNode];

                //Obliczenie obecnej górnej granicy
                int weightsToAddCount = 0;

                if (i != verticesToVisit.Length - 2)
                {
                    weightsToAddCount = verticesToVisit.Length - 2 - i;
                }

                int maxBound = accumulatedCosts[weightsToAddCount];

                if (currentPathWeight + maxBound > bestMinimalPathWeight)
                {
                    currentPathIsBetter = false;
                    break;
                }
            }

            if (currentPathIsBetter)
            {
                bestMinimalPathWeight = currentPathWeight;
                
                for (int i = 0; i < verticesToVisit.Length; i++)
                    bestPath[i] = verticesToVisit[i];
            }

        } while (NextPermutation(ref verticesToVisit));
        
        long bytesUsed = 0;
        
        try
        {
            bytesUsed = Buffer.ByteLength(adjacencyMatrix)
                        + Buffer.ByteLength(verticesToVisit)
                        + Buffer.ByteLength(sortedWeights)
                        + Buffer.ByteLength(accumulatedCosts)
                        + Buffer.ByteLength(bestPath);
        }
        catch (Exception e)
        {
            bytesUsed = 0;
        }
        
        return new ValueTuple<int, List<int>, long>(
            bestMinimalPathWeight,
            bestPath.Prepend(startingVertex).Append(startingVertex).ToList(),
            bytesUsed);
    }

    public static TspSolution SolveUsingBfs(MatrixData matrixData)
    {
        throw new NotImplementedException();
    }
    
    public static TspSolution SolveUsingLowCost(MatrixData matrixData)
    {
        throw new NotImplementedException();
    }
    
    static bool NextPermutation(ref int[] a)
    {
        if (a.Length < 2) return false;
        var k = a.Length - 2;

        while (k >= 0 && a[k].CompareTo( a[k+1]) >=0) k--;
        if( k < 0 )return false;

        var l = a.Length - 1;
        while (l > k && a[l].CompareTo(a[k]) <= 0) l--;

        var tmp = a[k];
        a[k] = a[l];
        a[l] = tmp;

        var i = k + 1;
        var j = a.Length - 1;
        while(i < j)
        {
            tmp = a[i];
            a[i] = a[j];
            a[j] = tmp;
            i++;
            j--;
        }

        return true;
    }

    private static bool ContainsEdge(List<Edge> edges, Edge target)
    {
        foreach (var edge in edges)
        {
            if (edge.Equals(target)) 
                return true;
        }

        return false;
    }
}