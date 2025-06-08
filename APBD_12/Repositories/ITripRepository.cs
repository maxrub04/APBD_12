using APBD_12.Models;

namespace APBD_12.Repositories;

public interface ITripRepository
{
    Task<Trip?> GetTripByIdAsync(int idTrip);
    Task<List<Trip>> GetTripsPaginatedAsync(int pageNum, int pageSize);
    Task<int> CountTripsAsync();
    Task<bool> IsClientAssignedToTripAsync(int idClient, int idTrip);
    Task AssignClientToTripAsync(ClientTrip clientTrip);
}