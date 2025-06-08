using System;
using System.Collections.Generic;

namespace APBD_12.DTOs;

public class TripsResponseDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripDto> Trips { get; set; }
}