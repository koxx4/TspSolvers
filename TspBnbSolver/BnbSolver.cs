using System.Diagnostics;
using Kaos.Combinatorics;
using TspUtils;

namespace TspBnbSolver;

public static class BnbSolver
{
    public static TspSolution SolveUsingDfs(MatrixData matrixData, int startingVertex)
    {
        Stopwatch stopwatch = new Stopwatch();

        //long memoryBefore = Process.GetCurrentProcess().WorkingSet64;
        stopwatch.Start();
        TspSolution tspSolution = DoSolveUsingDfs(matrixData, startingVertex);
        stopwatch.Stop();
       // long memoryAfter = Process.GetCurrentProcess().WorkingSet64;

        return new TspSolution(tspSolution.MinPathWeight, tspSolution.MinPath, stopwatch.Elapsed, 0);
    }
    
    private static TspSolution DoSolveUsingDfs(MatrixData matrixData, int startingVertex)
    {
        int numberOfVertices = matrixData.NumberOfVertices;
        int[,] adjacencyMatrix = matrixData.AdjacencyMatrixArray;

        //Wygeneruj listę wierzchołków, bez wierzchołka początkowego
        int[] verticesToVisit = Enumerable.Range(0, numberOfVertices)
            .Where(x => x != startingVertex)
            .ToArray();

        //Wygeneruj posortowane krawedzie wedlug kosztow od min do max
        List<Edge> sortedEdges = matrixData.GetSortedEdges();

        //Przygotuj generowanie permutacji
        //Permutation permutation = new Permutation(verticesToVisit.Length);

        //Przygotuj obecnie najlepsza sciezke i jej koszt
        int bestMinimalPathWeight = int.MaxValue;
        IEnumerable<int> bestPath = null;

        //Iteruj przez kazda mozliwa permutacje
        do
        {
            //Koszt obecnie badanej ścieżki
            int currentPathWeight = 0;
            //Czy obecna ścieżka jest lepsza od najlepszej znanej
            bool currentPathIsBetter = true;

            //Lista odwiedzonych krawedzi do generowania maxBound
            //List<Edge> visitedEdges = new(verticesToVisit.Length);

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
                int verticesLeft = (verticesToVisit.Length - 2) - i;
                int maxBound = 0;

                for (int j = 0; j < verticesLeft; j++)
                    maxBound += sortedEdges[j].Weight;

                if (currentPathWeight + maxBound >= bestMinimalPathWeight)
                {
                    currentPathIsBetter = false;
                    break;
                }

                //visitedEdges.Add(new Edge() {From = i, To = i + 1, Weight = currentPathWeight});
            }

            if (currentPathIsBetter)
            {
                //Console.WriteLine($"FOUND SHORTER PATH: {bestMinimalPathWeight}->{currentPathWeight}");
                bestMinimalPathWeight = currentPathWeight;
                bestPath = verticesToVisit;
            }

        } while (NextPermutation(ref verticesToVisit));

        return new TspSolution(bestMinimalPathWeight, bestPath.Prepend(startingVertex).Append(startingVertex).ToList());
    }

    public static TspSolution SolveUsingBfs(MatrixData matrixData)
    {
        throw new NotImplementedException();
    }
    
    public static TspSolution SolveUsingLowCost(MatrixData matrixData)
    {
        throw new NotImplementedException();
    }
    
    // private static bool NextPermutation(ref int[] array) {
    //     // Find longest non-increasing suffix
    //     int i = array.Length - 1;
    //     while (i > 0 && array[i - 1] >= array[i])
    //         i--;
    //     // Now i is the head index of the suffix
    //
    //     // Are we at the last permutation already?
    //     if (i <= 0)
    //         return false;
    //
    //     // Let array[i - 1] be the pivot
    //     // Find rightmost element greater than the pivot
    //     int j = array.Length - 1;
    //     while (array[j] <= array[i - 1])
    //         j--;
    //     // Now the value array[j] will become the new pivot
    //     // Assertion: j >= i
    //
    //     // Swap the pivot with j
    //     int temp = array[i - 1];
    //     array[i - 1] = array[j];
    //     array[j] = temp;
    //
    //     // Reverse the suffix
    //     j = array.Length - 1;
    //     while (i < j) {
    //         temp = array[i];
    //         array[i] = array[j];
    //         array[j] = temp;
    //         i++;
    //         j--;
    //     }
    //
    //     // Successfully computed the next permutation
    //     return true;
    // }
    
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