using Domain.Shared.Command;
using Domain.Shared.Exception;
using Domain.Shared.Value;

namespace Domain.OOP.Traditional.Customer
{
    public class Customer1
    {
        public ID CustomerId { get; set; }
        public EmailAddress EmailAddress { get; private set; }
        public Hash ConfirmationHash { get; private set; }
        public bool IsEmailAddressConfirmed { get; private set; }
        public PersonName Name { get; }

        private Customer1(ID id, EmailAddress emailAddress, Hash confirmationHash, PersonName name)
        {
            CustomerId = id;
            EmailAddress = emailAddress;
            ConfirmationHash = confirmationHash;
            Name = name;
        }

        public static Customer1 Register(RegisterCustomer command)
        {
            return new Customer1(
                command.CustomerId,
                command.EmailAddress,
                command.ConfirmationHash,
                command.Name
            );
        }

        public void ConfirmEmailAddress(ConfirmCustomerEmailAddress command)
        {
            if (!command.ConfirmationHash.Equals(ConfirmationHash))
            {
                throw new WrongConfirmationHashException();
            }

            IsEmailAddressConfirmed = true;
        }

        public void ChangeEmailAddress(ChangeCustomerEmailAddress command)
        {
            EmailAddress = command.EmailAddress;
            ConfirmationHash = command.ConfirmationHash;
            IsEmailAddressConfirmed = false;
        }
    }
}