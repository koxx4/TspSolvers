namespace TspUtils;

public static class TspSolutionToFileExporter
{
    public static void WriteToFullCv(string filename, string oldFile, TspSolution tspSolution, List<double> timeMeasurments, long memory = -1)
    {
        var file = File.Create(filename);

        TextWriter tw = new StreamWriter(file);

        var avg = Math.Round(timeMeasurments.Average(), 2, MidpointRounding.AwayFromZero);

        string mem = memory < 0 ? "" : $" {Convert.ToString(memory)} ";

        tw.Write($"{oldFile}{mem}{avg} {tspSolution}\n");

        foreach (var timeMeasurment in timeMeasurments)
        {
            tw.WriteLine(timeMeasurment);
        }

        tw.Flush();
        tw.Close();
    }
    
    public static void WriteToScientificGraphCv(string filename, List<List<int>> results)
    {
        FileStream file = new FileStream(filename, FileMode.Append);
        TextWriter textWriter = new StreamWriter(file);

        if (new FileInfo(filename).Length == 0)
            textWriter.Write("vertices,time\n");
        
        foreach (var result in results)
        {
            textWriter.WriteLine($"{result[0]},{result[1]}");
        }

        textWriter.Flush();
        textWriter.Close();
    }
    
    public static void WriteToScientificGraphWithMemoryCv(string filename, List<List<long>> results)
    {
        FileStream file = new FileStream(filename, FileMode.Append);
        TextWriter textWriter = new StreamWriter(file);

        if (new FileInfo(filename).Length == 0)
            textWriter.Write("vertices,time,memory\n");
        
        foreach (var result in results)
        {
            textWriter.WriteLine($"{result[0]},{result[1]},{result[2]}");
        }

        textWriter.Flush();
        textWriter.Close();
    }
}