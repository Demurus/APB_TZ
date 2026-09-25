using IlliaUlianych_APB_TZ.Entities;

namespace IlliaUlianych_APB_TZ.DTO.Rooms;

public class CreateConferenceRoomResult
{
    public ConferenceRoomResultType Type { get; }
    public ConferenceRoom? Room { get; }

    private CreateConferenceRoomResult(
        ConferenceRoomResultType type,
        ConferenceRoom? room = null)
    {
        Type = type;
        Room = room;
    }

    public static CreateConferenceRoomResult Success(ConferenceRoom room)
        => new(ConferenceRoomResultType.Success, room);
    
    public static CreateConferenceRoomResult ServicesUnavailable()
        => new(ConferenceRoomResultType.ServiceUnavailable);
}