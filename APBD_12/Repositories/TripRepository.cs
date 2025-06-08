using APBD_12.Data;
using APBD_12.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_12.Repositories;

public class TripRepository : ITripRepository
{
    private readonly Apbd12Context _context;

    public TripRepository(Apbd12Context context)
    {
        _context = context;
    }

    public async Task<Trip?> GetTripByIdAsync(int idTrip)
    {
        return await _context.Trips
            .Include(t => t.IdCountries)
            .FirstOrDefaultAsync(t => t.IdTrip == idTrip);
    }

    public async Task<List<Trip>> GetTripsPaginatedAsync(int pageNum, int pageSize)
    {
        return await _context.Trips
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation)
            .OrderByDescending(t => t.DateFrom)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountTripsAsync()
    {
        return await _context.Trips.CountAsync();
    }

    public async Task<bool> IsClientAssignedToTripAsync(int idClient, int idTrip)
    {
        return await _context.ClientTrips
            .AnyAsync(ct => ct.IdClient == idClient && ct.IdTrip == idTrip);
    }

    public async Task AssignClientToTripAsync(ClientTrip clientTrip)
    {
        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();
    }
}