using System.Collections.Generic;

namespace APBD_12.Models;

public class Country
{
    public int IdCountry { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();
}
