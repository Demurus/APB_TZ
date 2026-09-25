using IlliaUlianych_APB_TZ.DTO.Rooms;
using IlliaUlianych_APB_TZ.Services;
using Microsoft.AspNetCore.Mvc;

namespace IlliaUlianych_APB_TZ.Controllers;

[ApiController]
[Route("api/rooms")]
public class ConferenceRoomsController : ControllerBase
{
    private readonly IConferenceRoomService _conferenceRoomService;

    public ConferenceRoomsController(IConferenceRoomService conferenceRoomService)
    {
        _conferenceRoomService = conferenceRoomService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRoom(CreateConferenceRoomRequest request)
    {
        var result = await _conferenceRoomService.CreateRoomAsync(request);
        
        switch (result.Type)
        {
            case ConferenceRoomResultType.Success:
                if (result.Room == null)
                {
                    throw new InvalidOperationException(
                        "Successful create room result does not contain a room.");
                }
                
                return Created(
                    $"/api/rooms/{result.Room.Id}",
                    new
                    {
                        result.Room.Id
                    });
            case ConferenceRoomResultType.ServiceUnavailable:
                return BadRequest(
                    "One or more services do not exist.");
            default:
                throw new InvalidOperationException();
        }
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRoom(
        int id,
        UpdateConferenceRoomRequest request)
    {
        var result = await _conferenceRoomService.UpdateRoomAsync(id, request);

        switch (result)
        {
            case ConferenceRoomResultType.Success:
                return NoContent();
            case ConferenceRoomResultType.RoomNotFound:
                return NotFound();
            case ConferenceRoomResultType.ServiceUnavailable:
                return BadRequest("One or more services do not exist.");
            default:
                throw new InvalidOperationException();
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var result = await _conferenceRoomService.DeleteRoomAsync(id);

        switch (result)
        {
            case ConferenceRoomResultType.Success:
                return NoContent();
            case ConferenceRoomResultType.RoomNotFound:
                return NotFound();
            case ConferenceRoomResultType.Conflict:
                return Conflict("Conference room cannot be deleted because it has bookings.");
            default:
                throw new InvalidOperationException();
        }
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

        var result = await _conferenceRoomService.GetAvailableRoomsAsync(request);
        return Ok(result);
    }
}