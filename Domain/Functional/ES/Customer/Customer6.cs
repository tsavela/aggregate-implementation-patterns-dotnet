using System.Collections.Generic;
using Domain.Shared.Command;
using Domain.Shared.Event;

namespace Domain.Functional.ES.Customer
{
    public class Customer6 {
        public static CustomerRegistered Register(RegisterCustomer command) {
            return CustomerRegistered.Build(command.CustomerId, command.EmailAddress, command.ConfirmationHash, command.Name);
        }

        public static List<IEvent> ConfirmEmailAddress(List<IEvent> eventStream, ConfirmCustomerEmailAddress command) {
            var current = CustomerState.Reconstitute(eventStream);

            var generatedEvents = new List<IEvent>();

            if (!command.ConfirmationHash.Equals(current.ConfirmationHash))
            {
                generatedEvents.Add(CustomerEmailAddressConfirmationFailed.Build(command.CustomerId));
            }
            else
            {
                if (!current.IsEmailAddressConfirmed)
                    generatedEvents.Add(CustomerEmailAddressConfirmed.Build(command.CustomerId));
            }

            return generatedEvents;
        }

        public static List<IEvent> ChangeEmailAddress(List<IEvent> eventStream, ChangeCustomerEmailAddress command) {
            var current = CustomerState.Reconstitute(eventStream);

            var generatedEvents = new List<IEvent>();

            if (!command.EmailAddress.Equals(current.EmailAddress))
            {
                generatedEvents.Add(CustomerEmailAddressChanged.Build(command.CustomerId, command.EmailAddress, command.ConfirmationHash));
            }

            return generatedEvents;
        }
    }
}
