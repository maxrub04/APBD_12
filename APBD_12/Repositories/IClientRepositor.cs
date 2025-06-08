using APBD_12.Models;

namespace APBD_12.Repositories;

public interface IClientRepository
{
    Task<Client?> GetClientByIdAsync(int idClient);
    Task<Client?> GetClientByPeselAsync(string pesel);
    Task<bool> ClientHasTripsAsync(int idClient);
    Task DeleteClientAsync(Client client);
    Task<Client> AddClientAsync(Client client);
}