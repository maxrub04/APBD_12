using System;
using System.Linq;
using System.Threading.Tasks;
using APBD_12.Data;
using APBD_12.DTOs;
using APBD_12.Models;

namespace APBD_12.Services;

using APBD_12.Data;
using APBD_12.DTOs;
using APBD_12.Models;
using Microsoft.EntityFrameworkCore;


public class TripService : ITripService
{
    private readonly TripsDbContext _context;

    public TripService(TripsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedTripResultDto> GetTripsAsync(int page, int pageSize)
    {
        var query = _context.Trips
            .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.CountryTrips)
                .ThenInclude(ct => ct.IdCountryNavigation)
            .OrderByDescending(t => t.DateFrom);

        int totalCount = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var trips = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripResponseDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.CountryTrips.Select(c => new CountryDto
                {
                    Name = c.IdCountryNavigation.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(c => new ClientDto
                {
                    FirstName = c.IdClientNavigation.FirstName,
                    LastName = c.IdClientNavigation.LastName
                }).ToList()
            }).ToListAsync();

        return new PagedTripResultDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = totalPages,
            Trips = trips
        };
    }

    public async Task<string> AssignClientToTripAsync(int idTrip, ClientTripDto dto)
    {
        if (await _context.Clients.AnyAsync(c => c.Pesel == dto.Pesel))
            throw new Exception("Client with given PESEL already exists.");

        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
            throw new Exception("Trip not found.");

        if (trip.DateFrom < DateTime.Now)
            throw new Exception("Cannot register for a past trip.");

        var client = new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var clientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();

        return "Client assigned successfully.";
    }
}
