using APBD_12.Data;
using APBD_12.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_12.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly Apbd12Context _context;
    
    public ClientRepository(Apbd12Context context)
    {
        _context = context;
    }

    public async Task<Client?> GetClientByIdAsync(int idClient)
    {
        return await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);
    }

    public async Task<Client?> GetClientByPeselAsync(string pesel)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.Pesel == pesel);
    }

    public async Task<bool> ClientHasTripsAsync(int idClient)
    {
        return await _context.ClientTrips
            .AnyAsync(ct => ct.IdClient == idClient);
    }

    public async Task DeleteClientAsync(Client client)
    {
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task<Client> AddClientAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }
}