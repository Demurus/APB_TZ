using IlliaUlianych_APB_TZ.DTO.Rooms;

namespace IlliaUlianych_APB_TZ.Services;

public interface IConferenceRoomService
{
    /// <summary>
    /// Creates a conference room and attaches it to the selected catalog services
    /// </summary>
    Task<CreateConferenceRoomResult> CreateRoomAsync(
        CreateConferenceRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing conference room and optionally replaces its available services
    /// </summary>
    Task<ConferenceRoomResultType> UpdateRoomAsync(
        int id,
        UpdateConferenceRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a conference room when it has no booking history
    /// </summary>
    Task<ConferenceRoomResultType> DeleteRoomAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds rooms according to capacity and booking availability constraints
    /// </summary>
    Task<IReadOnlyCollection<AvailableConferenceRoomResponse>> GetAvailableRoomsAsync(
        SearchAvailableRoomsRequest request,
        CancellationToken cancellationToken = default);
}