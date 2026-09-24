using IlliaUlianych_APB_TZ.Data;
using IlliaUlianych_APB_TZ.DTO.Rooms;
using IlliaUlianych_APB_TZ.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IlliaUlianych_APB_TZ.Controllers;

[ApiController]
[Route("api/rooms")]
public class ConferenceRoomsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ConferenceRoomsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRoom(
        CreateConferenceRoomRequest request)
    {
        var requestedServiceIds =
            request.AvailableServiceIds.Distinct().ToList();

        var services = await _dbContext.RoomServices
            .Where(service =>
                requestedServiceIds.Contains(service.Id))
            .ToListAsync();

        if (services.Count != requestedServiceIds.Count)
        {
            return BadRequest(
                "One or more services do not exist.");
        }

        var room = new ConferenceRoom(
            request.Name,
            request.Capacity,
            request.BaseHourPrice,
            services);

        _dbContext.ConferenceRooms.Add(room);

        await _dbContext.SaveChangesAsync();

        return Created(
            $"/api/rooms/{room.Id}",
            new
            {
                room.Id
            });
    }
}