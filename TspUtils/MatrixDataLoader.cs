using System.Text.RegularExpressions;

namespace TspUtils
{
    public static class MatrixDataLoader
    {
        private static readonly string[] _requiredTspProperties = new[] {
                "TYPE:TSP",
                "EDGE_WEIGHT_TYPE:EXPLICIT",
                "DIMENSION",
                "EDGE_WEIGHT_FORMAT",
                "EDGE_WEIGHT_SECTION"
            };

        private static readonly string[] _requiredAtspProperties = new[] {
                "TYPE:ATSP",
                "EDGE_WEIGHT_TYPE:EXPLICIT",
                "DIMENSION",
                "EDGE_WEIGHT_FORMAT",
                "EDGE_WEIGHT_SECTION"
            };

        private static readonly string[] _validEdgeWeightFormats = new[] {
                "FULL_MATRIX",
                "LOWER_DIAG_ROW",
                //"UPPER_DIAG_ROW",
                "UPPER_ROW"
            };

        private static readonly Regex sWhitespace = new(@"\s+");


        public static MatrixData? DispatcherLoader(string fileName)
        {
            if (fileName.EndsWith(".txt"))
            {
                return LoadFromTxtFile(fileName);
            }
            else if (fileName.EndsWith(".tsp"))
            {
                return LoadFromTspFile(fileName);
            }
            else if (fileName.EndsWith(".atsp"))
            {
                return LoadFromAtspFile(fileName);
            }

            WriteOutput("Nieprawidłowy typ pliku.");

            return null;
        }

        public static MatrixData? LoadFromTspFile(string fileName)
        {
            try
            {
                string[] fileLines = File.ReadAllLines(fileName);         
                
                if (!IsTspFileValid(fileLines))
                {
                    WriteOutput("Plik ma złą zawartość!");

                    return null;
                }               

                int numberOfVertices = GetDimensionPropertyValue(fileLines);
                string edgeWeightFormatType = GetEdgeWeightType(fileLines);
                string[] edgeWeightSectionDataLines = GetEdgeWeightSectionLines(fileLines);

                List<List<int>> vertices = edgeWeightFormatType switch
                {
                    "FULL_MATRIX" => ParseFullMatrix(edgeWeightSectionDataLines, numberOfVertices),
                    "LOWER_DIAG_ROW" => ParseLowerDiagRow(edgeWeightSectionDataLines, numberOfVertices),
                    "UPPER_DIAG_ROW" => ParseUpperRow(edgeWeightSectionDataLines, numberOfVertices),
                    "UPPER_ROW" => ParseUpperRow(edgeWeightSectionDataLines, numberOfVertices),
                    _ => new List<List<int>> { new() }
                };

                WriteOutput("Wczytano plik!");

                return new MatrixData(numberOfVertices, vertices);
            }
            catch (Exception ex)
            {
                WriteOutput($"Nie udało się wczytać {fileName}");

                return null;
            }
        }

        public static MatrixData? LoadFromAtspFile(string fileName)
        {
            try
            {
                string[] fileLines = File.ReadAllLines(fileName);

                if (!IsAtspFileValid(fileLines))
                {
                    WriteOutput("Plik ma złą zawartość!");

                    return null;
                }

                int numberOfVertices = GetDimensionPropertyValue(fileLines);
                string edgeWeightFormatType = GetEdgeWeightType(fileLines);
                string[] edgeWeightSectionDataLines = GetEdgeWeightSectionLines(fileLines);

                List<List<int>> vertices = edgeWeightFormatType switch
                {
                    "FULL_MATRIX" => ParseFullMatrix(edgeWeightSectionDataLines, numberOfVertices),
                    _ => new List<List<int>> { new List<int> { 0 } }
                };

                WriteOutput("Wczytano plik!");

                return new MatrixData(numberOfVertices, vertices);
            }
            catch
            {
                WriteOutput($"Nie udało się wczytać {fileName}");

                return null;
            }
        }

        public static MatrixData? LoadFromTxtFile(string fileName)
        {
            try
            {
                string[] fileLines = File.ReadAllLines(fileName);

                int vertexCount = Convert.ToInt32(fileLines[0]);

                List<List<int>> matrix = ParseAdjacencyMatrix(fileLines, vertexCount, skipLines: 1);

                return new MatrixData(vertexCount, matrix);
            }
            catch (Exception e)
            {
                WriteOutput($"Błąd przy parsowaniu {fileName}.");
                
                return null;
            }
        }

        private static List<List<int>> ParseAdjacencyMatrix(string[] fileLines, int vertexCount, int skipLines)
        {
            int[] rows = fileLines
                .Skip(1)
                .Select(fileLine => fileLine
                    .Trim()
                    .Split(' ')
                    .Where(value => int.TryParse(value, out _))
                    .Select(value => Convert.ToInt32(value))
                    .ToArray()
                ).SelectMany(x => x).ToArray();

            List<List<int>> matrix = new(vertexCount);

            for (int i = 0; i < rows.Length; i += vertexCount)
            {
                List<int> row = new(vertexCount);
                for (int j = 0; j < vertexCount; j++)
                {
                    row.Add(rows[j + i]);
                }
                matrix.Add(row);
            }

            return matrix;
        }

        private static List<List<int>> ParseFullMatrix(string[] fileLines, int numberOfVertices)
        {
            int[,] list = new int[numberOfVertices, numberOfVertices];

            int[] values = fileLines
                .Select(fileLine => fileLine
                    .Trim()
                    .Split(' ')
                    .Where(value => int.TryParse(value, out _))
                    .Select(value => Convert.ToInt32(value))
                    .ToArray()
                ).SelectMany(x => x).ToArray();

            int valuesIndex = 0;

            for (int i = 0; i < numberOfVertices; i++)
            {
                for (int j = 0; j < numberOfVertices; j++)
                {
                    list[i, j] = values[valuesIndex];
                    valuesIndex++;
                }
            }

            return To2DList(list);
        }

        private static List<List<int>> ParseLowerDiagRow(string[] fileLines, int numberOfVertices)
        {
            int line = 0;

            int[,] list = new int[numberOfVertices, numberOfVertices];

            int value_iterator = 0;
            int[] values = fileLines
                .Select(fileLine => fileLine
                    .Trim()
                    .Split(' ')
                    .Where(value => int.TryParse(value, out _))
                    .Select(value => Convert.ToInt32(value))
                    .ToArray()
                ).SelectMany(x => x).ToArray();

            for (int i = 0; i < numberOfVertices; i++)
            {
                for (int j = 0; j <= line; j++)
                {
                    list[i, j] = values[value_iterator];
                    value_iterator++;
                }

                for (int j = line + 1; j < numberOfVertices; j++)
                {
                    list[line, j] = -1;
                }
                
                line++;
            }

            return To2DList(list);
        }

        private static List<List<int>> ParseUpperRow(string[] fileLines, int numberOfVertices)
        {
            int line = numberOfVertices - 2;

            int[,] list = new int[numberOfVertices, numberOfVertices];
            
            int value_iterator = 0;
            int[] values = fileLines
                .Select(fileLine => fileLine
                    .Trim()
                    .Split(' ')
                    .Where(value => int.TryParse(value, out _))
                    .Select(value => Convert.ToInt32(value))
                    .ToArray()
                ).SelectMany(x => x).ToArray();

            for (int i = 0; i < numberOfVertices; i++)
            {
                for (int j = 0; j <= line; j++)
                {
                    list[i, j] = values[value_iterator];
                    value_iterator++;
                }
                
                for (int j = numberOfVertices - 1; j > line; j--)
                {
                    list[i, j] = -1;
                }
                
                line--;
            }

            return To2DList(list);
        }

        private static List<List<int>> To2DList(int[,] ints)
        {
            List<List<int>> list = new(ints.GetLength(0));

            for (int i = 0; i < ints.GetLength(0); i++)
            {
                list.Add(new List<int>());
                for (int j = 0; j < ints.GetLength(1); j++)
                {
                    list[i].Add(ints[i, j]);
                }
            }

            return list;
        }

        private static int GetDimensionPropertyValue(string[] fileLines)
        {
            string value = fileLines.Where(line => line.Trim().StartsWith("DIMENSION")).First();

            value = RemoveWhitespace(value).Replace("DIMENSION:", "").Trim();

            return Convert.ToInt32(value);
        }

        private static string GetEdgeWeightType(string[] fileLines)
        {
            string value = fileLines.Where(line => line.Trim().StartsWith("EDGE_WEIGHT_FORMAT")).First();

            return RemoveWhitespace(value).Replace("EDGE_WEIGHT_FORMAT:", "").Trim();
        }

        private static string[] GetEdgeWeightSectionLines(string[] fileLines)
        {
            for (int i = 0; i < fileLines.Length; i++)
            {
                if (fileLines[i] == "EDGE_WEIGHT_SECTION")
                {
                    return fileLines.Skip(i + 1).ToArray();
                }
            }

            return Array.Empty<string>();
        }

        private static bool IsTspFileValid(string[] fileLines)
        {
            List<string> notIncludedProperties = GetNotIncludedProperties(fileLines, _requiredTspProperties);

            notIncludedProperties
                .ForEach(property => WriteOutput($"W pliku brakuje: {property}"));

            bool hasRequiredProperties = notIncludedProperties.Count == 0;

            bool hasValidEdgeWeightFormat = isEdgeWeightFormatValid(fileLines);

            return hasRequiredProperties && hasValidEdgeWeightFormat;
        }

        private static bool IsAtspFileValid(string[] fileLines)
        {
            List<string> notIncludedProperties = GetNotIncludedProperties(fileLines, _requiredAtspProperties);

            notIncludedProperties
                .ForEach(property => WriteOutput($"W pliku brakuje: {property}"));

            bool hasRequiredProperties = notIncludedProperties.Count == 0;

            bool hasValidEdgeWeightFormat = isEdgeWeightFormatValid(fileLines);
                
            return hasRequiredProperties && hasValidEdgeWeightFormat;
        }

        private static bool isEdgeWeightFormatValid(string[] fileLines)
        {
            string type = GetEdgeWeightType(fileLines);

            bool valid = _validEdgeWeightFormats
                .Contains(type);

            if (!valid) 
                WriteOutput($"Co to za macierz: {type} ? Dozwolone typy EDGE_WEIGHT_FORMAT to {string.Join(',', _validEdgeWeightFormats)}");

            return valid;
        }

        private static List<string> GetNotIncludedProperties(string[] dataLines, string[] properties)
        {
            List<string> notIncludedProperties = new();

            foreach (string property in properties)
            {
                bool isPropertyIncluded = false;

                foreach (string dataLine in dataLines)
                {
                    if (RemoveWhitespace(dataLine).StartsWith(property))
                    {
                        isPropertyIncluded = true;

                        break;
                    }
                }

                if (!isPropertyIncluded)
                {
                    notIncludedProperties.Add(property);
                }
            }

            return notIncludedProperties;
        }

        private static void WriteOutput(string msg)
        {
            Console.WriteLine(msg);
        }

        private static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        private static string RemoveWhitespace(string input)
        {
            return ReplaceWhitespace(input, "");
        }
    }
}
