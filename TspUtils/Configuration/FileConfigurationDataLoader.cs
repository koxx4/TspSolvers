namespace TspUtils.Configuration;

public abstract class FileConfigurationDataLoader<T, TC> : IConfigurationDataLoader<T, TC>
    where T : ConfigurationLine
    where TC : ConfigurationData<T>
{
    protected string _configurationFilePath;
    
    protected FileConfigurationDataLoader(string path)
    {
        CheckIfFileExists(path);
        
        _configurationFilePath = path;
    }
    
    public virtual TC LoadConfiguration()
    {
        return ReadFromFile();
    }

    private void CheckIfFileExists(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Nie mogłem znaleźć pliku inicjalizującego {path}!");
            Console.WriteLine("Wciśnij jakiś przycisk by zakończyć.");

            Console.ReadKey(true);

            throw new ArgumentException($"Brak pliku {path}");
        }
    }

    private TC ReadFromFile()
    {
        string[] fileLines = File.ReadAllLines(_configurationFilePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        TC parsedLines; 
        
        try
        {
            parsedLines = ParseFileLines(fileLines);
        }
        catch (Exception e)
        {
            Console.WriteLine("Błąd podczas parsowania ustawień programu!");
            Console.WriteLine(e.Message, e.StackTrace);
            throw;
        }
        
        return parsedLines;
    }

    protected abstract TC ParseFileLines(string[] fileLines);
}