using System.Collections.Generic;
using Domain.Shared.Event;
using Domain.Shared.Value;

namespace Domain.Functional.ES.Customer
{
    public class CustomerState {
        public EmailAddress EmailAddress { get; private set; }
        public Hash ConfirmationHash { get; private set; }
        public PersonName Name { get; private set; }
        public bool IsEmailAddressConfirmed { get; private set; }

        private CustomerState() {}

        public static CustomerState Reconstitute(List<IEvent> events) {
            var customer = new CustomerState();

            customer.Apply(events);

            return customer;
        }

        public void Apply(List<IEvent> events) {
            foreach (var @event in events)
            {
                if (@event is CustomerRegistered) {
                    // TODO
                    continue;
                }

                if (@event is CustomerEmailAddressConfirmed) {
                    // TODO
                    continue;
                }

                if (@event is CustomerEmailAddressChanged) {
                    // TODO
                }
            }
        }
    }
}
