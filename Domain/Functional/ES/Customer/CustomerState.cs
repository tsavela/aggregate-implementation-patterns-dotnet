using System.Collections.Generic;
using Domain.Shared.Event;
using Domain.Shared.Value;

namespace Domain.Functional.ES.Customer
{
    public class CustomerState {
        EmailAddress emailAddress;
        Hash confirmationHash;
        PersonName name;
        bool isEmailAddressConfirmed;

        private CustomerState() {}

        public static CustomerState Reconstitute(List<IEvent> events) {
            var customer = new CustomerState();

            customer.Apply(events);

            return customer;
        }

        void Apply(List<IEvent> events) {
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
