using Domain.Shared.Event;
using Domain.Shared.Value;

public class CustomerEmailAddressChanged : IEvent {
    public ID CustomerId { get; }
    public EmailAddress EmailAddress { get; }
    public Hash ConfirmationHash { get; }

    private CustomerEmailAddressChanged(ID customerID, EmailAddress emailAddress, Hash confirmationHash)
    {
        CustomerId = customerID;
        EmailAddress = emailAddress;
        ConfirmationHash = confirmationHash;
    }

    public static CustomerEmailAddressChanged Build(ID customerID, EmailAddress emailAddress, Hash confirmationHash) {
        return new CustomerEmailAddressChanged(customerID, emailAddress, confirmationHash);
    }
}
