
using APBD_12.Data;
using APBD_12.DTOs;
using APBD_12.Models;
using APBD_12.Repositories;
using Microsoft.EntityFrameworkCore;

namespace APBD_12.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly IClientRepository _clientRepository;

    public TripService(ITripRepository tripRepository, IClientRepository clientRepository)
    {
        _tripRepository = tripRepository;
        _clientRepository = clientRepository;
    }

    public async Task<TripsResponseDto> GetTripsAsync(int pageNum, int pageSize)
    {
        var totalTrips = await _tripRepository.CountTripsAsync();
        var totalPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        var trips = await _tripRepository.GetTripsPaginatedAsync(pageNum, pageSize);

        var tripDtos = trips.Select(t => new TripDto
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
        }).ToList();

        return new TripsResponseDto
        {
            PageNum = pageNum,
            PageSize = pageSize,
            AllPages = totalPages,
            Trips = tripDtos
        };
    }

    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var client = await _clientRepository.GetClientByIdAsync(idClient);
        if (client == null)
        {
            throw new InvalidOperationException("Client not found");
        }

        if (await _clientRepository.ClientHasTripsAsync(idClient))
        {
            throw new InvalidOperationException("Client has assigned trips");
        }

        await _clientRepository.DeleteClientAsync(client);
        return true;
    }

    public async Task AssignClientToTripAsync(AssignClientToTripDto dto)
    {
        var trip = await _tripRepository.GetTripByIdAsync(dto.IdTrip);
        if (trip == null)
        {
            throw new InvalidOperationException("Trip not found");
        }

        if (trip.DateFrom <= DateTime.Now)
        {
            throw new InvalidOperationException("Trip has already started or completed");
        }

        var client = await _clientRepository.GetClientByPeselAsync(dto.Pesel);
        if (client == null)
        {
            client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };
            client = await _clientRepository.AddClientAsync(client);
        }

        if (await _tripRepository.IsClientAssignedToTripAsync(client.IdClient, dto.IdTrip))
        {
            throw new InvalidOperationException("Client is already assigned to this trip");
        }

        var clientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = dto.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        await _tripRepository.AssignClientToTripAsync(clientTrip);
    }
}