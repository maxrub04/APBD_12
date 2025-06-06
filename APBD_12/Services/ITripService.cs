using System.Threading.Tasks;
using APBD_12.DTOs;

using APBD_12.DTOs;

namespace APBD_12.Services;

public interface ITripService
{
    Task<PagedTripResultDto> GetTripsAsync(int page, int pageSize);
    Task<string> AssignClientToTripAsync(int idTrip, ClientTripDto dto);
}
