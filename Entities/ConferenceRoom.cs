namespace IlliaUlianych_APB_TZ.Entities;

public class ConferenceRoom
{
    private readonly List<RoomService> _availableServices = new List<RoomService>();
    
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int Capacity { get; private set; }
    public decimal BaseHourPrice { get; private set; }
    public IReadOnlyCollection<RoomService> AvailableServices => _availableServices;

    private ConferenceRoom()
    {
    }

    public ConferenceRoom(
        string name,
        int capacity,
        decimal baseHourPrice,
        IEnumerable<RoomService>? availableServices = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Name cannot be null or empty.",
                nameof(name));
        }

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Capacity must be greater than zero.");
        }

        if (baseHourPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseHourPrice),
                "Price cannot be negative.");
        }

        Name = name;
        Capacity = capacity;
        BaseHourPrice = baseHourPrice;

        if (availableServices is not null)
        {
            _availableServices.AddRange(availableServices);
        }
    }
}