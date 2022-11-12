using System.Text;

namespace TspUtils
{
    public struct Edge
    {
        public int From;
        public int To;
        public int Weight;

        public bool Equals(Edge other)
        {
            return From == other.From && To == other.To && Weight == other.Weight;
        }
    }
    
    public class MatrixData
    {
        public int NumberOfVertices { get; }
        public List<List<int>> AdjacencyMatrix { get; }
        public int[,] AdjacencyMatrixArray { get; }

        public MatrixData(int numberOfVertices, List<List<int>> adjacencyMatrix)
        {
            this.NumberOfVertices = numberOfVertices;
            this.AdjacencyMatrix = adjacencyMatrix;
            this.AdjacencyMatrixArray = ToAdjacencyMatrixArray(adjacencyMatrix);
        }

        public List<int> GetSortedPathWeights()
        {
            List<int> weights = new(NumberOfVertices * NumberOfVertices - NumberOfVertices);
            
            for (int i = 0; i < NumberOfVertices; i++)
            {
                for (int j = 0; j < NumberOfVertices; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    
                    weights.Add(AdjacencyMatrixArray[i, j]);
                }
            }
            
            weights.Sort();

            return weights;
        }
        
        public List<Edge> GetSortedEdges()
        {
            List<Edge> weights = new(NumberOfVertices * NumberOfVertices - NumberOfVertices);
            
            for (int i = 0; i < NumberOfVertices; i++)
            {
                for (int j = 0; j < NumberOfVertices; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    int weight = AdjacencyMatrixArray[i, j];
                    
                    weights.Add(new Edge() {From = j, To = i, Weight = weight});
                }
            }
            
            weights.Sort((edge1, edge2) => edge1.Weight - edge2.Weight );

            return weights;
        }

        private int[,] ToAdjacencyMatrixArray(List<List<int>> adjacencyMatrix)
        {
            int matrixSize = adjacencyMatrix.Count;

            int[,] matrix = new int[matrixSize, matrixSize];

            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    matrix[i, j] = adjacencyMatrix[i][j];
                }
            }

            return matrix;
        }
        
        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"Wierzchołków: {NumberOfVertices}");

            foreach (var row in AdjacencyMatrix)
            {
                foreach (var col in row)
                {
                    stringBuilder.Append(Convert.ToString(col));
                    stringBuilder.Append(' ');
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
