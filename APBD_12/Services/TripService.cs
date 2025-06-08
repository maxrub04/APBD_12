
using APBD_12.Data;
using APBD_12.DTOs;
using APBD_12.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_12.Services;

public class TripService : ITripService
{
    private readonly Apbd12Context _context;

    public TripService(Apbd12Context context)
    {
        _context = context;
    }

    public async Task<TripsResponseDto> GetTripsAsync(int pageNum, int pageSize)
    {
        var totalTrips = await _context.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        var trips = await _context.Trips
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
            .OrderByDescending(t => t.DateFrom)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto { Name = c.Name }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            })
            .ToListAsync();

        return new TripsResponseDto
        {
            PageNum = pageNum,
            PageSize = pageSize,
            AllPages = totalPages,
            Trips = trips
        };
    }

    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            return false;
        }

        if (client.ClientTrips.Any())
        {
            throw new InvalidOperationException("Client has assigned trips and cannot be deleted.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task AssignClientToTripAsync(AssignClientToTripDto dto)
    {
        // Check if trip exists and is in the future
        var trip = await _context.Trips.FindAsync(dto.IdTrip);
        if (trip == null)
        {
            throw new ArgumentException("Trip not found.");
        }

        if (trip.DateFrom <= DateTime.Now)
        {
            throw new ArgumentException("Trip has already started or completed.");
        }

        // Check if client exists by PESEL
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);

        if (client == null)
        {
            // Create new client
            client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        // Check if client is already assigned to this trip
        var existingAssignment = await _context.ClientTrips
            .AnyAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == dto.IdTrip);

        if (existingAssignment)
        {
            throw new InvalidOperationException("Client is already assigned to this trip.");
        }

        // Assign client to trip
        var clientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = dto.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();
    }
}