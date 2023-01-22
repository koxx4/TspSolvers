using TspUtils.Configuration;

namespace TspAntColony.Configuration;

public class AcConfigurationData : ConfigurationData<AcConfigurationDataLine>
{
    public AcConfigurationData(List<AcConfigurationDataLine> configurationLines) : base(configurationLines)
    {
    }
}