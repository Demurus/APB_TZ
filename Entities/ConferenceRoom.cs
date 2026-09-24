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
        ValidateName(name);
        ValidateCapacity(capacity);
        ValidatePrice(baseHourPrice);
        
        Name = name;
        Capacity = capacity;
        BaseHourPrice = baseHourPrice;

        if (availableServices is not null)
        {
            _availableServices.AddRange(availableServices);
        }
    }

    public void ChangeName(string newName)
    {
        ValidateName(newName);
        Name = newName;
    }

    public void ChangeCapacity(int newCapacity)
    {
        ValidateCapacity(newCapacity);
        Capacity = newCapacity;
    }

    public void ChangeBaseHourPrice(decimal newBaseHourPrice)
    {
        ValidatePrice(newBaseHourPrice);
        BaseHourPrice = newBaseHourPrice;
    }

    public void AddService(RoomService service)
    {
        if (service == null)
        {
            throw new ArgumentException(
                "Service cannot be null",
                nameof(service));
        }

        if (_availableServices.Any(rs => rs.Id == service.Id))
        {
            return;
        }

        _availableServices.Add(service);
    }

    public void RemoveService(RoomService service)
    {
        if (service == null)
        {
            throw new ArgumentException(
                "Service cannot be null",
                nameof(service));
        }

        if (_availableServices.All(rs => rs.Id != service.Id))
        {
            return;
        }

        _availableServices.Remove(service);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Name cannot be null or empty.",
                nameof(name));
        }
    }

    private void ValidateCapacity(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Capacity must be greater than zero.");
        }
    }

    private void ValidatePrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Price cannot be negative.");
        }
    }
}