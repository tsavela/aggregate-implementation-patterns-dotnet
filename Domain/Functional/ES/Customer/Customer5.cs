using System.Collections.Generic;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;

namespace Domain.Functional.ES.Customer
{
    public class Customer5 {
        public static CustomerRegistered Register(RegisterCustomer command)
        {
            return CustomerRegistered.Build(command.CustomerId, command.EmailAddress, command.ConfirmationHash, command.Name);
        }

        public static List<IEvent> ConfirmEmailAddress(List<IEvent> eventStream, ConfirmCustomerEmailAddress command) {
            bool isEmailAddressConfirmed = false;
            Hash confirmationHash = null;
            foreach (var @event in eventStream)
            {
                if (@event is CustomerRegistered customerRegisteredEvent)
                {
                    isEmailAddressConfirmed = false;
                    confirmationHash = customerRegisteredEvent.ConfirmationHash;
                } else if (@event is CustomerEmailAddressConfirmed customerEmailAddressConfirmedEvent) {
                    isEmailAddressConfirmed = true;
                } else if (@event is CustomerEmailAddressChanged customerEmailAddressChangedEvent) {
                    isEmailAddressConfirmed = false;
                    confirmationHash = customerEmailAddressChangedEvent.ConfirmationHash;
                }
            }

            var generatedEvents = new List<IEvent>();

            if (!confirmationHash.Equals(command.ConfirmationHash))
            {
                generatedEvents.Add(CustomerEmailAddressConfirmationFailed.Build(command.CustomerId));
            }
            else
            {
                if (!isEmailAddressConfirmed)
                {
                    generatedEvents.Add(CustomerEmailAddressConfirmed.Build(command.CustomerId));
                }
            }

            return generatedEvents;
        }

        public static List<IEvent> ChangeEmailAddress(List<IEvent> eventStream, ChangeCustomerEmailAddress command) {
            EmailAddress emailAddress = null;
            foreach (var @event in eventStream)
            {
                if (@event is CustomerRegistered customerRegisteredEvent)
                {
                    emailAddress = customerRegisteredEvent.EmailAddress;
                } else if (@event is CustomerEmailAddressChanged customerEmailAddressChangedEvent)
                {
                    emailAddress = customerEmailAddressChangedEvent.EmailAddress;
                }
            }

            var generatedEvents = new List<IEvent>();

            if (emailAddress == null || !emailAddress.Equals(command.EmailAddress))
            {
                generatedEvents.Add(CustomerEmailAddressChanged.Build(command.CustomerId, command.EmailAddress, command.ConfirmationHash));
            }

            return generatedEvents;
        }
    }
}
