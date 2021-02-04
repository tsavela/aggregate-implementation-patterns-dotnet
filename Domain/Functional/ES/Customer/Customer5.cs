using System.Collections.Generic;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;

namespace Domain.Functional.ES.Customer
{
    public class Customer5 {
        public static CustomerRegistered Register(RegisterCustomer command) {
            return null; // TODO
        }

        public static List<IEvent> ConfirmEmailAddress(List<IEvent> eventStream, ConfirmCustomerEmailAddress command) {
            bool isEmailAddressConfirmed = false;
            Hash confirmationHash = null;
            foreach (var @event in eventStream)
            {
                if (@event is CustomerRegistered) {
                    // TODO
                } else if (@event is CustomerEmailAddressConfirmed) {
                    // TODO
                } else if (@event is CustomerEmailAddressChanged) {
                    // TODO
                }
            }

            // TODO

            return new List<IEvent>(); // TODO
        }

        public static List<IEvent> ChangeEmailAddress(List<IEvent> eventStream, ChangeCustomerEmailAddress command) {
            EmailAddress emailAddress = null;
            foreach (var @event in eventStream)
            {
                if (@event is CustomerRegistered) {
                    // TODO
                } else if (@event is CustomerEmailAddressChanged) {
                    // TODO
                }
            }

            // TODO

            return new List<IEvent>(); // TODO
        }
    }
}
