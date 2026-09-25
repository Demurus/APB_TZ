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
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRoom(
        int id,
        UpdateConferenceRoomRequest request)
    {
        var room = await _dbContext.ConferenceRooms
            .Include(room => room.AvailableServices)
            .FirstOrDefaultAsync(room => room.Id == id);
        
        if (room is null)
        {
            return NotFound();
        }
        
        if (request.Name is not null)
        {
            room.ChangeName(request.Name);
        }

        if (request.Capacity.HasValue)
        {
            room.ChangeCapacity(request.Capacity.Value);
        }

        if (request.BaseHourPrice.HasValue)
        {
            room.ChangeBaseHourPrice(request.BaseHourPrice.Value);
        }

        if (request.AvailableServiceIds is not null)
        {
            var requestedServiceIds = request.AvailableServiceIds
                .Distinct()
                .ToList();
        
            var services = await _dbContext.RoomServices
                .Where(service => requestedServiceIds.Contains(service.Id))
                .ToListAsync();
        
            if (services.Count != requestedServiceIds.Count)
            {
                return BadRequest("One or more services do not exist.");
            }
            
            var servicesToRemove = room.AvailableServices
                .Where(existing => !requestedServiceIds.Contains(existing.Id))
                .ToList();

            foreach (var service in servicesToRemove)
            {
                room.RemoveService(service);
            }
            
            foreach (var service in services)
            {
                if (room.AvailableServices.All(existing => existing.Id != service.Id))
                {
                    room.AddService(service);
                }
            }
           
        }
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _dbContext.ConferenceRooms
            .FirstOrDefaultAsync(room => room.Id == id);

        if (room is null)
        {
            return NotFound();
        }

        var hasBookings = await _dbContext.Bookings
            .AnyAsync(booking => booking.RoomId == id);

        if (hasBookings)
        {
            return Conflict("Conference room cannot be deleted because it has bookings.");
        }

        _dbContext.ConferenceRooms.Remove(room);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRooms(
        [FromQuery] SearchAvailableRoomsRequest request)
    {
        if (request.Capacity <= 0)
        {
            return BadRequest("Capacity must be greater than zero.");
        }

        if (request.EndTime <= request.StartTime)
        {
            return BadRequest("End time must be later than start time.");
        }

        var rooms = await _dbContext.ConferenceRooms
            .Where(room => room.Capacity >= request.Capacity)
            .Where(room => !_dbContext.Bookings.Any(booking =>
                booking.RoomId == room.Id &&
                booking.StartTime < request.EndTime &&
                booking.EndTime > request.StartTime))
            .Include(room => room.AvailableServices)
            .AsNoTracking()
            .ToListAsync();

        var responses =
            new List<AvailableConferenceRoomResponse>(rooms.Count);

        foreach (var conferenceRoom in rooms)
        {
            var availableServicesIds = new List<int>();

            foreach (var roomService in conferenceRoom.AvailableServices)
            {
                availableServicesIds.Add(roomService.Id);
            }

            var response = new AvailableConferenceRoomResponse
            {
                Id = conferenceRoom.Id,
                Name = conferenceRoom.Name,
                Capacity = conferenceRoom.Capacity,
                BaseHourPrice = conferenceRoom.BaseHourPrice,
                AvailableServices = availableServicesIds
            };

            responses.Add(response);
        }

        return Ok(responses);
    }
}