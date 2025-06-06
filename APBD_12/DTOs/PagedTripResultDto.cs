namespace APBD_12.DTOs;
public class PagedTripResultDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripResponseDto> Trips { get; set; } = new();
}
