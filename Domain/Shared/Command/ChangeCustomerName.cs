using Domain.Shared.Value;

namespace Domain.Shared.Command
{
    public class ChangeCustomerName {
        public ID CustomerId { get; }
        public PersonName Name { get; }

        private ChangeCustomerName(string customerID, string givenName, string familyName) {
            CustomerId = ID.Build(customerID);
            Name = PersonName.Build(givenName, familyName);
        }

        public static ChangeCustomerName Build(string customerID, string givenName, string familyName) {
            return new ChangeCustomerName(customerID, givenName, familyName);
        }
    }
}
