using Domain.Shared.Value;

namespace Domain.Shared.Command
{
    public class ChangeCustomerEmailAddress {
        public ID CustomerId { get; }
        public EmailAddress EmailAddress { get; }
        public Hash ConfirmationHash { get; }

        private ChangeCustomerEmailAddress(string customerID, string emailAddress)
        {
            CustomerId = ID.Build(customerID);
            EmailAddress = EmailAddress.Build(emailAddress);
            ConfirmationHash = Hash.Generate();
        }

        public static ChangeCustomerEmailAddress Build(string customerID, string emailAddress) {
            return new ChangeCustomerEmailAddress(customerID, emailAddress);
        }
    }
}
