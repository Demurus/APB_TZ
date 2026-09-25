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
    
    /// <summary>
    /// Creates a conference room and assigns catalog services available in this room.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRoom(
        CreateConferenceRoomRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _conferenceRoomService.CreateRoomAsync(
            request,
            cancellationToken);
        
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
    
    /// <summary>
    /// Updates conference room details and optionally replaces available service IDs.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRoom(
        int id,
        UpdateConferenceRoomRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _conferenceRoomService.UpdateRoomAsync(
            id,
            request,
            cancellationToken);

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
    
    /// <summary>
    /// Deletes a conference room if it has no booking history.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRoom(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _conferenceRoomService.DeleteRoomAsync(
            id,
            cancellationToken);

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
    
    /// <summary>
    /// Returns rooms that match the requested capacity and are free for the selected time interval.
    /// </summary>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRooms(
        [FromQuery] SearchAvailableRoomsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Capacity <= 0)
        {
            return BadRequest("Capacity must be greater than zero.");
        }

        if (request.EndTime <= request.StartTime)
        {
            return BadRequest("End time must be later than start time.");
        }

        var result = await _conferenceRoomService.GetAvailableRoomsAsync(
            request,
            cancellationToken);
        return Ok(result);
    }
}