using Domain.Shared.Command;
using Domain.Shared.Exception;

namespace Domain.Functional.Traditional.Customer
{
    public class Customer2
    {
        public static CustomerState Register(RegisterCustomer command)
        {
            return new CustomerState(
                command.CustomerId,
                command.EmailAddress,
                command.ConfirmationHash,
                command.Name
            );
        }

        public static CustomerState ConfirmEmailAddress(CustomerState current, ConfirmCustomerEmailAddress command)
        {
            if (!command.ConfirmationHash.Equals(current.ConfirmationHash))
            {
                throw new WrongConfirmationHashException();
            }


            return new CustomerState(
                current.CustomerId,
                current.EmailAddress,
                current.ConfirmationHash,
                current.Name,
                true
            );
        }

        public static CustomerState ChangeEmailAddress(CustomerState current, ChangeCustomerEmailAddress command)
        {
            return new CustomerState(
                current.CustomerId,
                command.EmailAddress,
                command.ConfirmationHash,
                current.Name,
                false
            );
        }
    }
}
