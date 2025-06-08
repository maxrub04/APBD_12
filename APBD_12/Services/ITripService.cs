using System.Threading.Tasks;
using APBD_12.DTOs;

using APBD_12.DTOs;

namespace APBD_12.Services;

public interface ITripService
{
    Task<TripsResponseDto> GetTripsAsync(int pageNum, int pageSize);
    Task<bool> DeleteClientAsync(int idClient);
    Task AssignClientToTripAsync(AssignClientToTripDto dto);
}