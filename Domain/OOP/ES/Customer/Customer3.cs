using System.Collections.Generic;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;

namespace Domain.OOP.ES.Customer
{
    public class Customer3 {
        public EmailAddress EmailAddress { get; private set; }
        public Hash ConfirmationHash { get; private set; }
        public bool IsEmailAddressConfirmed { get; private set; }
        public PersonName Name { get; private set; }

        private Customer3() {
        }

        public static CustomerRegistered Register(RegisterCustomer command) {
            return CustomerRegistered.Build(command.CustomerId, command.EmailAddress, command.ConfirmationHash, command.Name);
        }

        public static Customer3 Reconstitute(List<IEvent> events) {
            Customer3 customer = new Customer3();

            customer.Apply(events);

            return customer;
        }

        public List<IEvent> ConfirmEmailAddress(ConfirmCustomerEmailAddress command) {
            var events = new List<IEvent>();

            if (Equals(command.ConfirmationHash, ConfirmationHash))
            {
                if(!IsEmailAddressConfirmed)
                    events.Add(CustomerEmailAddressConfirmed.Build(command.CustomerId));
            }
            else
                events.Add(CustomerEmailAddressConfirmationFailed.Build(command.CustomerId));

            return events;
        }

        public List<IEvent> ChangeEmailAddress(ChangeCustomerEmailAddress command)
        {
            var events = new List<IEvent>();

            if(!Equals(command.EmailAddress, EmailAddress))
            {
                events.Add(CustomerEmailAddressChanged.Build(command.CustomerId, command.EmailAddress, command.ConfirmationHash));
            }

            return events;
        }

        public void Apply(List<IEvent> events) {
            foreach (var @event in events)
            {
                Apply(@event);
            }
        }

        public void Apply(IEvent @event) {
            if (@event is CustomerRegistered customerRegisteredEvent)
            {
                Name = customerRegisteredEvent.Name;
                ConfirmationHash = customerRegisteredEvent.ConfirmationHash;
                EmailAddress = customerRegisteredEvent.EmailAddress;
                IsEmailAddressConfirmed = false;
            } else if (@event is CustomerEmailAddressConfirmed customerEmailAddressConfirmedEvent) {
                IsEmailAddressConfirmed = true;
            } else if (@event is CustomerEmailAddressChanged customerEmailAddressChangedEvent)
            {
                IsEmailAddressConfirmed = false;
                EmailAddress = customerEmailAddressChangedEvent.EmailAddress;
                ConfirmationHash = customerEmailAddressChangedEvent.ConfirmationHash;
            }
        }
    }
}
