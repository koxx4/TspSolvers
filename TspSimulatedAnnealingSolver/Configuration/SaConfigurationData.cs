using TspUtils.Configuration;

namespace TspSimulatedAnnealingSolver.Configuration;

public class SaConfigurationData : ConfigurationData<SaConfigurationLine>
{
    public SaConfigurationData(List<SaConfigurationLine> configurationLines) : base(configurationLines)
    {
    }
}