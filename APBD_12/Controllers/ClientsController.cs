using APBD_12.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_12.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ITripService _tripService;

    public ClientsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        try
        {
            var result = await _tripService.DeleteClientAsync(idClient);
            if (!result)
            {
                return NotFound("Client not found.");
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}