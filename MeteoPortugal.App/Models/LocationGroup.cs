using System.Collections.ObjectModel;

namespace MeteoPortugal.App.Models;

public sealed class LocationGroup : ObservableCollection<WeatherLocation>
{
    public LocationGroup(string name, IEnumerable<WeatherLocation> locations)
        : base(locations)
    {
        Name = name;
    }

    public string Name { get; }
}
