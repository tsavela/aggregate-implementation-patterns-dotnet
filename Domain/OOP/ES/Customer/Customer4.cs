
using System.Collections.Generic;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;

namespace Domain.OOP.ES.Customer
{
    public class Customer4 {
        public EmailAddress EmailAddress { get; }
        public Hash ConfirmationHash { get; }
        public bool IsEmailAddressConfirmed { get; }
        public PersonName Name { get; }

        private readonly List<IEvent> _recordedEvents;

        private Customer4() {
            _recordedEvents = new List<IEvent>();
        }

        public static Customer4 Register(RegisterCustomer command) {
            Customer4 customer = new Customer4();

            // TODO

            return customer;
        }

        public static Customer4 Reconstitute(List<IEvent> events) {
            var customer = new Customer4();

            customer.Apply(events);

            return customer;
        }

        public void ConfirmEmailAddress(ConfirmCustomerEmailAddress command) {
            // TODO
        }

        public void ChangeEmailAddress(ChangeCustomerEmailAddress command) {
            // TODO
        }

        public List<IEvent> GetRecordedEvents() {
            return _recordedEvents;
        }

        private void RecordThat(IEvent @event) {
            _recordedEvents.Add(@event);
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
