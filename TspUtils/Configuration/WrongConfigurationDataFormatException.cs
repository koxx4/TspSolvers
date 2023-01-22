namespace TspUtils.Configuration;

public class WrongConfigurationDataFormatException : Exception
{
    public WrongConfigurationDataFormatException(string? message) : base(message)
    {
    }
}