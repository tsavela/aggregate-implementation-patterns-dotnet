using Domain.Shared.Value;

namespace Domain.Shared.Command
{
    public class ConfirmCustomerEmailAddress {
        public ID CustomerId { get; }
        public Hash ConfirmationHash { get; }

        private ConfirmCustomerEmailAddress(string customerID, string confirmationHash) {
            CustomerId = ID.Build(customerID);
            ConfirmationHash = Hash.Build(confirmationHash);
        }

        public static ConfirmCustomerEmailAddress Build(string customerID, string confirmationHash) {
            return new ConfirmCustomerEmailAddress(customerID, confirmationHash);
        }
    }
}
