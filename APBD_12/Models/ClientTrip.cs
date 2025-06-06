using System;

namespace APBD_12.Models;

public class ClientTrip
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? PaymentDate { get; set; }

    public Client IdClientNavigation { get; set; } = null!;
    public Trip IdTripNavigation { get; set; } = null!;
}
