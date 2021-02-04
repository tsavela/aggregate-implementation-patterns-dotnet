using Domain.Shared.Event;
using Domain.Shared.Value;

public class CustomerEmailAddressConfirmationFailed : IEvent
{
    public ID CustomerId { get; }

    private CustomerEmailAddressConfirmationFailed(ID customerID)
    {
        CustomerId = customerID;
    }

    public static CustomerEmailAddressConfirmationFailed Build(ID customerID)
    {
        return new CustomerEmailAddressConfirmationFailed(customerID);
    }
}
