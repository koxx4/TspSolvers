namespace TspUtils;

public struct ScientificCsvDataLineWithErrorRate
{
    public int VerticesCount;
    public long Time;
    public double ErrorRate;
}


public static class TspSolutionToFileExporter
{
    public static void WriteToFullCv(string filename, string oldFile, TspSolution tspSolution, List<double> timeMeasurments, long memory = -1)
    {
        var file = File.Create(filename);

        TextWriter tw = new StreamWriter(file);

        var avg = Math.Round(timeMeasurments.Average(), 2, MidpointRounding.AwayFromZero);

        string mem = memory < 0 ? " " : $" {Convert.ToString( (ulong) memory)} ";

        tw.Write($"{oldFile}{mem}{avg} {tspSolution}\n");

        foreach (var timeMeasurment in timeMeasurments)
        {
            tw.WriteLine(timeMeasurment);
        }

        tw.Flush();
        tw.Close();
    }
    
    public static void WriteToFullCvWithErrors(string filename, string oldFile, List<TspSolution> tspSolutions, List<double> errors, long memory = -1)
    {
        var file = File.Create(filename);

        TextWriter tw = new StreamWriter(file);
        tw.Write("#nazwa instancji,czas działania,błąd [%],znaleziony koszt,znalezione rozwiązanie");
        
        string mem = memory < 0 ? "," : $",{Convert.ToString( (ulong) memory)},";

        int i = 0;
        foreach (var tspSolution in tspSolutions)
        {
            string error = Math.Round(errors[i], 3, MidpointRounding.AwayFromZero).ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
            tw.Write($"{oldFile}{mem}{tspSolution.ExecutionTime.Milliseconds},{error},{tspSolution}\n");
            i++;
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
    
    public static void WriteToScientificGraphCvWithErrorRate(string filename, ScientificCsvDataLineWithErrorRate lineWithErrorRate)
    {
        FileStream file = new FileStream(filename, FileMode.Append);
        TextWriter textWriter = new StreamWriter(file);

        if (new FileInfo(filename).Length == 0)
            textWriter.Write("vertices,time,error\n");

        textWriter.WriteLine($"{lineWithErrorRate.VerticesCount},{lineWithErrorRate.Time},{lineWithErrorRate.ErrorRate.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)}");

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
            textWriter.WriteLine($"{result[0]},{result[1]},{(ulong)result[2]}");
        }

        textWriter.Flush();
        textWriter.Close();
    }
}