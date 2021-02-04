using Domain.Shared.Value;

namespace Domain.Functional.Traditional.Customer
{
    public class CustomerState {
        public ID CustomerId { get; }
        public EmailAddress EmailAddress { get; }
        public Hash ConfirmationHash { get; }
        public PersonName Name { get; }
        public bool IsEmailAddressConfirmed { get; }

        public CustomerState(ID id, EmailAddress emailAddress, Hash confirmationHash, PersonName name, bool isEmailAddressConfirmed = false)
        {
            CustomerId = id;
            ConfirmationHash = confirmationHash;
            Name = name;
            IsEmailAddressConfirmed = isEmailAddressConfirmed;
            EmailAddress = emailAddress;
        }
    }
}
