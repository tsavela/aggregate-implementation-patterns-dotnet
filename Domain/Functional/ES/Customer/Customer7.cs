using System.Collections.Generic;
using Domain.Shared.Command;
using Domain.Shared.Event;

namespace Domain.Functional.ES.Customer
{
    public class Customer7 {
        public static CustomerRegistered Register(RegisterCustomer command) {
            return null; // TODO
        }

        public static List<IEvent> ConfirmEmailAddress(CustomerState current, ConfirmCustomerEmailAddress command) {
            // TODO

            return new List<IEvent>(); // TODO
        }

        public static List<IEvent> ChangeEmailAddress(CustomerState current, ChangeCustomerEmailAddress command) {
            // TODO

            return new List<IEvent>(); // TODO
        }
    }
}
