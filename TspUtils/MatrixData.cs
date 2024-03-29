﻿using System.Collections;
using System.ComponentModel;
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
        
        public List<int> GetSortedWeights()
        {
            List<int> weights = new();

            for (int i = 0; i < NumberOfVertices; i++)
            {
                for (int j = 0; j < NumberOfVertices; j++)
                {
                    if (i != j)
                    {
                        weights.Add(AdjacencyMatrixArray[i, j]);
                    }
                }
            }
            
            weights.Sort();

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
