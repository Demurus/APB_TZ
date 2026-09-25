using IlliaUlianych_APB_TZ.DTO.Rooms;

namespace IlliaUlianych_APB_TZ.Services;

public interface IConferenceRoomService
{
    Task<CreateConferenceRoomResult> CreateRoomAsync(CreateConferenceRoomRequest request);
    Task<ConferenceRoomResultType> UpdateRoomAsync(int id, UpdateConferenceRoomRequest request);
    Task<ConferenceRoomResultType> DeleteRoomAsync(int id);
    Task<IReadOnlyCollection<AvailableConferenceRoomResponse>> GetAvailableRoomsAsync(SearchAvailableRoomsRequest request);
}