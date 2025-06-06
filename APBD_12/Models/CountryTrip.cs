namespace APBD_12.Models;

public class CountryTrip
{
    public int IdCountry { get; set; }
    public int IdTrip { get; set; }

    public Country IdCountryNavigation { get; set; } = null!;
    public Trip IdTripNavigation { get; set; } = null!;
}
