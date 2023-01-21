namespace TspSimulatedAnnealingSolver.Algorithm.Neighbourhood;

public interface INeighbourGenerationStrategy
{
    int[] GenerateNextNeighbour(int[] currentSolution);
}

public class TwoSwapNeighbourGenerationStrategy : INeighbourGenerationStrategy
{
    private readonly Random _random = new();

    public int[] GenerateNextNeighbour(int[] currentSolution)
    {
        int[] newSolution = new int[currentSolution.Length];
        currentSolution.CopyTo(newSolution, 0);
        
        if (currentSolution.Length < 2)
        {
            return currentSolution;
        }
        
        if (currentSolution.Length == 2)
        {
            (newSolution[0], newSolution[1]) =
                (newSolution[1], newSolution[0]);
        }

        int firstIndexToSwap = _random.Next(0, currentSolution.Length);
        int secondIndexToSwap = _random.Next(0, currentSolution.Length);

        while (secondIndexToSwap == firstIndexToSwap)
        {
            secondIndexToSwap = _random.Next(0, currentSolution.Length);
        }

        (newSolution[firstIndexToSwap], newSolution[secondIndexToSwap]) =
            (newSolution[secondIndexToSwap], newSolution[firstIndexToSwap]);

        return newSolution;
    }
}

public class TwoSwapNNeighbourGenerationStrategy : INeighbourGenerationStrategy
{
    private readonly Random _random = new();

    public int[] GenerateNextNeighbour(int[] currentSolution)
    {
        int[] newSolution = new int[currentSolution.Length];
        currentSolution.CopyTo(newSolution, 0);
        
        if (currentSolution.Length < 2)
        {
            return currentSolution;
        }
        
        if (currentSolution.Length == 2)
        {
            (newSolution[0], newSolution[1]) =
                (newSolution[1], newSolution[0]);
        }

        int firstIndexToSwap = _random.Next(0, currentSolution.Length);
        int secondIndexToSwap;
        
        if (firstIndexToSwap == currentSolution.Length - 1)
        {
            secondIndexToSwap = firstIndexToSwap - 1;
        }
        else
        {
            secondIndexToSwap = firstIndexToSwap + 1;
        }
        
        (newSolution[firstIndexToSwap], newSolution[secondIndexToSwap]) =
            (newSolution[secondIndexToSwap], newSolution[firstIndexToSwap]);

        return newSolution;
    }
}

public class TwoInsertNeighbourGenerationStrategy : INeighbourGenerationStrategy
{
    private readonly Random _random = new();
    
    public int[] GenerateNextNeighbour(int[] currentSolution)
    {
        int[] newSolution = new int[currentSolution.Length];
        currentSolution.CopyTo(newSolution, 0);
        
        if (currentSolution.Length < 2)
        {
            return currentSolution;
        }
        
        if (currentSolution.Length == 2)
        {
            (newSolution[0], newSolution[1]) =
                (newSolution[1], newSolution[0]);
        }
        
        int firstIndexToSwap = _random.Next(0, currentSolution.Length);
        int secondIndexToSwap = _random.Next(0, currentSolution.Length);

        while (secondIndexToSwap == firstIndexToSwap)
        {
            secondIndexToSwap = _random.Next(0, currentSolution.Length);
        }

        int lowerIndex = Math.Min(firstIndexToSwap, secondIndexToSwap);
        int upperIndex = Math.Max(firstIndexToSwap, secondIndexToSwap);

        int valueUnderUpperIndex = newSolution[upperIndex];

        //Przesuwanie tablicy w prawo
        //Scislej mowiac przez nadpisywanie tablicy iterujac od gornego indeksu
        for (int i = upperIndex; i > lowerIndex; i--)
        {
            newSolution[i] = newSolution[i - 1];
        }

        newSolution[lowerIndex] = valueUnderUpperIndex;

        return newSolution;
    }
}

public class ReverseNeighbourGenerationStrategy : INeighbourGenerationStrategy
{
    private readonly Random _random = new();
    
    public int[] GenerateNextNeighbour(int[] currentSolution)
    {
        int[] newSolution = new int[currentSolution.Length];
        currentSolution.CopyTo(newSolution, 0);
        
        if (currentSolution.Length < 2)
        {
            return currentSolution;
        }
        else if (currentSolution.Length == 2)
        {
            (newSolution[0], newSolution[1]) =
                (newSolution[1], newSolution[0]);
        }
        
        int firstIndexToSwap = _random.Next(0, currentSolution.Length);
        int secondIndexToSwap = _random.Next(0, currentSolution.Length);

        while (secondIndexToSwap == firstIndexToSwap)
        {
            secondIndexToSwap = _random.Next(0, currentSolution.Length);
        }

        int lowerIndex = Math.Min(firstIndexToSwap, secondIndexToSwap);
        int upperIndex = Math.Max(firstIndexToSwap, secondIndexToSwap);

        while (lowerIndex < upperIndex)
        {
            (newSolution[lowerIndex], newSolution[upperIndex]) = 
                (newSolution[upperIndex], newSolution[lowerIndex]);

            lowerIndex++;
            upperIndex--;
        }

        return newSolution;
    }
}