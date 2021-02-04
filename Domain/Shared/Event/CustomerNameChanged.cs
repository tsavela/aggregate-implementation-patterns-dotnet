using Domain.Shared.Event;
using Domain.Shared.Value;

public class CustomerNameChanged : IEvent {
    public ID CustomerId { get; }
    public PersonName Name { get; }

    private CustomerNameChanged(ID customerID, PersonName name)
    {
        CustomerId = customerID;
        Name = name;
    }

    public static CustomerNameChanged Build(ID customerID, PersonName name) {
        return new CustomerNameChanged(customerID, name);
    }
}
