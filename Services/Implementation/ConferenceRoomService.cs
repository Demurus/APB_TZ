using IlliaUlianych_APB_TZ.Data;
using IlliaUlianych_APB_TZ.DTO.Rooms;
using IlliaUlianych_APB_TZ.Entities;
using Microsoft.EntityFrameworkCore;

namespace IlliaUlianych_APB_TZ.Services;

internal class ConferenceRoomService : IConferenceRoomService
{
    private readonly ApplicationDbContext _dbContext;

    public ConferenceRoomService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<CreateConferenceRoomResult> CreateRoomAsync(
        CreateConferenceRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var requestedServiceIds =
            request.AvailableServiceIds.Distinct().ToList();

        var services = await _dbContext.RoomServices
            .Where(service =>
                requestedServiceIds.Contains(service.Id))
            .ToListAsync(cancellationToken);

        if (services.Count != requestedServiceIds.Count)
        {
            return CreateConferenceRoomResult.ServicesUnavailable();
        }

        var room = new ConferenceRoom(
            request.Name,
            request.Capacity,
            request.BaseHourPrice,
            services);

        _dbContext.ConferenceRooms.Add(room);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return CreateConferenceRoomResult.Success(room);
    }

    public async Task<ConferenceRoomResultType> UpdateRoomAsync(
        int id,
        UpdateConferenceRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await _dbContext.ConferenceRooms
            .Include(room => room.AvailableServices)
            .FirstOrDefaultAsync(room => room.Id == id, cancellationToken);
        
        if (room is null)
        {
            return ConferenceRoomResultType.RoomNotFound;
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
                .ToListAsync(cancellationToken);
        
            if (services.Count != requestedServiceIds.Count)
            {
                return ConferenceRoomResultType.ServiceUnavailable;
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
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ConferenceRoomResultType.Success;
    }

    public async Task<ConferenceRoomResultType> DeleteRoomAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var room = await _dbContext.ConferenceRooms
            .FirstOrDefaultAsync(room => room.Id == id, cancellationToken);

        if (room is null)
        {
            return ConferenceRoomResultType.RoomNotFound;
        }

        var hasBookings = await _dbContext.Bookings
            .AnyAsync(booking => booking.RoomId == id, cancellationToken);

        if (hasBookings)
        {
            return ConferenceRoomResultType.Conflict;
        }

        _dbContext.ConferenceRooms.Remove(room);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ConferenceRoomResultType.Success;
    }

    public async Task<IReadOnlyCollection<AvailableConferenceRoomResponse>> GetAvailableRoomsAsync(
        SearchAvailableRoomsRequest request,
        CancellationToken cancellationToken = default)
    {
        var rooms = await _dbContext.ConferenceRooms
            .Where(room => room.Capacity >= request.Capacity)
            .Where(room => !_dbContext.Bookings.Any(booking =>
                booking.RoomId == room.Id &&
                booking.StartTime < request.EndTime &&
                booking.EndTime > request.StartTime))
            .Include(room => room.AvailableServices)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var responses = new List<AvailableConferenceRoomResponse>(rooms.Count);

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

        return responses;
    }
}