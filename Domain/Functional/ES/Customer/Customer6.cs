using System.Collections.Generic;
using Domain.Shared.Command;
using Domain.Shared.Event;

namespace Domain.Functional.ES.Customer
{
    public class Customer6 {
        public static CustomerRegistered Register(RegisterCustomer command) {
            return null; // TODO
        }

        public static List<IEvent> ConfirmEmailAddress(List<IEvent> eventStream, ConfirmCustomerEmailAddress command) {
            var current = CustomerState.Reconstitute(eventStream);

            // TODO

            return new List<IEvent>(); // TODO
        }

        public static List<IEvent> ChangeEmailAddress(List<IEvent> eventStream, ChangeCustomerEmailAddress command) {
            var current = CustomerState.Reconstitute(eventStream);

            // TODO

            return new List<IEvent>(); // TODO
        }
    }
}
