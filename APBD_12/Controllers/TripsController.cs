using System;
using System.Threading.Tasks;
using APBD_12.Data;
using APBD_12.DTOs;
using APBD_12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace APBD_12.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly TripsDbContext _context;

    public TripsController(TripsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PagedTripResultDto>> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
                Countries = t.CountryTrips
                    .Select(c => new CountryDto { Name = c.IdCountryNavigation.Name })
                    .ToList(),
                Clients = t.ClientTrips
                    .Select(c => new ClientDto()
                    {
                        FirstName = c.IdClientNavigation.FirstName,
                        LastName = c.IdClientNavigation.LastName
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(new PagedTripResultDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = totalPages,
            Trips = trips
        });
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientTripDto dto)
    {
        if (await _context.Clients.AnyAsync(c => c.Pesel == dto.Pesel))
            return Conflict("Client with given PESEL already exists.");

        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
            return NotFound("Trip not found.");

        if (trip.DateFrom < DateTime.Now)
            return BadRequest("Cannot register for a past trip.");

        var client = new Models.Client
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

        return Ok("Client assigned successfully.");
    }
}
