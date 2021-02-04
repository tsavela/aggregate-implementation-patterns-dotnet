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
            return null; // TODO
        }

        public static Customer3 Reconstitute(List<IEvent> events) {
            Customer3 customer = new Customer3();

            customer.Apply(events);

            return customer;
        }

        public List<IEvent> ConfirmEmailAddress(ConfirmCustomerEmailAddress command) {
            // TODO

            return new List<IEvent>(); // TODO
        }

        public List<IEvent> ChangeEmailAddress(ChangeCustomerEmailAddress command) {
            // TODO

            return new List<IEvent>(); // TODO
        }

        public void Apply(List<IEvent> events) {
            foreach (var @event in events)
            {
                Apply(@event);
            }
        }

        public void Apply(IEvent @event) {
            if (@event is CustomerRegistered) {
                // TODO
            } else if (@event is CustomerEmailAddressConfirmed) {
                // TODO
            } else if (@event is CustomerEmailAddressChanged) {
                // TODO
            }
        }
    }
}
