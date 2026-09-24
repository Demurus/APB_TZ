
namespace IlliaUlianych_APB_TZ.Entities;

public class Booking
{
    private readonly List<RoomService> _selectedServices = new();

    public int Id { get; private set; }

    public int RoomId { get; private set; }
    public ConferenceRoom Room { get; private set; } = null!;

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public decimal TotalPrice { get; private set; }

    public IReadOnlyCollection<RoomService> SelectedServices =>
        _selectedServices;

    private Booking()
    {
    }

    public Booking(
        ConferenceRoom room,
        DateTime startTime,
        TimeSpan duration,
        decimal totalPrice,
        IEnumerable<RoomService>? selectedServices = null)
    {
        ArgumentNullException.ThrowIfNull(room);

        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(duration),
                "Booking duration must be greater than zero.");
        }

        if (totalPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalPrice),
                "Total price cannot be negative.");
        }

        Room = room;
        RoomId = room.Id;

        StartTime = startTime;
        EndTime = startTime.Add(duration);
        TotalPrice = totalPrice;

        if (selectedServices is not null)
        {
            _selectedServices.AddRange(selectedServices);
        }
    }
}