namespace APBD_12.Models;

public class Trip
{
    public int IdTrip { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }

    public ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();
    public ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();
}
