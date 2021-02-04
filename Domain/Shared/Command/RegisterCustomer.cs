using Domain.Shared.Value;

namespace Domain.Shared.Command
{
    public class RegisterCustomer {
        public ID CustomerId { get; }
        public EmailAddress EmailAddress { get; }
        public Hash ConfirmationHash { get; }
        public PersonName Name { get; }

        private RegisterCustomer(string emailAddress, string givenName, string familyName) {
            CustomerId = ID.Generate();
            ConfirmationHash = Hash.Generate();
            EmailAddress = EmailAddress.Build(emailAddress);
            Name = PersonName.Build(givenName, familyName);
        }

        public static RegisterCustomer Build(string emailAddress, string givenName, string familyName) {
            return new RegisterCustomer(emailAddress, givenName, familyName);
        }
    }
}
