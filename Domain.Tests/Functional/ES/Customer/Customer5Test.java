using System.Collections.Generic;
using Domain.Functional.ES.Customer;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;
using Xunit;
using Xunit.Sdk;

namespace Domain.Tests.Functional.ES.Customer
{
    public class Customer5Test {
        private ID customerID;
        private EmailAddress emailAddress;
        private EmailAddress changedEmailAddress;
        private Hash confirmationHash;
        private Hash wrongConfirmationHash;
        private Hash changedConfirmationHash;
        private PersonName name;
        private List<IEvent> eventStream;
        private CustomerRegistered customerRegistered;
        private List<IEvent> recordedEvents;

        public Customer5Test()
        {
            customerID = ID.Generate();
            emailAddress = EmailAddress.Build("john@doe.com");
            changedEmailAddress = EmailAddress.Build("john+changed@doe.com");
            confirmationHash = Hash.Generate();
            wrongConfirmationHash = Hash.Generate();
            changedConfirmationHash = Hash.Generate();
            name = PersonName.Build("John", "Doe");
            eventStream = new List<IEvent>();
            recordedEvents = new List<IEvent>();
        }

        [Fact]
        public void RegisterCustomer() {
            WHEN_RegisterCustomer();
            THEN_CustomerRegistered();
        }

        [Fact]
        public void ConfirmEmailAddress() {
            GIVEN_CustomerRegistered();
            WHEN_ConfirmEmailAddress_With(confirmationHash);
            THEN_EmailAddressConfirmed();
        }

        [Fact]
        public void confirmEmailAddress_withWrongConfirmationHash() {
            GIVEN_CustomerRegistered();
            WHEN_ConfirmEmailAddress_With(wrongConfirmationHash);
            THEN_EmailAddressConfirmationFailed();
        }

        [Fact]
        public void confirmEmailAddress_whenItWasAlreadyConfirmed() {
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasConfirmed();
            WHEN_ConfirmEmailAddress_With(confirmationHash);
            THEN_NothingShouldHappen();
        }

        [Fact]
        public void confirmEmailAddress_withWrongConfirmationHash_whenItWasAlreadyConfirmed() {
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasConfirmed();
            WHEN_ConfirmEmailAddress_With(wrongConfirmationHash);
            THEN_EmailAddressConfirmationFailed();
        }

        [Fact]
        public void changeEmailAddress() {
            GIVEN_CustomerRegistered();
            WHEN_ChangeEmailAddress_With(changedEmailAddress);
            THEN_EmailAddressChanged();
        }

        [Fact]
        public void changeEmailAddress_withUnchangedEmailAddress() {
            // Given
            GIVEN_CustomerRegistered();
            WHEN_ChangeEmailAddress_With(emailAddress);
            THEN_NothingShouldHappen();
        }

        [Fact]
        public void changeEmailAddress_whenItWasAlreadyChanged() {
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasChanged();
            WHEN_ChangeEmailAddress_With(changedEmailAddress);
            THEN_NothingShouldHappen();
        }

        [Fact]
        public void confirmEmailAddress_whenItWasPreviouslyConfirmedAndThenChanged() {
            // Given
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasConfirmed();
            __and_EmailAddressWasChanged();
            WHEN_ConfirmEmailAddress_With(changedConfirmationHash);
            THEN_EmailAddressConfirmed();
        }

        /**
     * Methods for GIVEN
     */

        private void GIVEN_CustomerRegistered() {
            eventStream.Add(CustomerRegistered.Build(customerID, emailAddress, confirmationHash, name));
        }

        private void __and_EmailAddressWasConfirmed() {
            eventStream.Add(CustomerEmailAddressConfirmed.Build(customerID));
        }

        private void __and_EmailAddressWasChanged() {
            eventStream.Add(CustomerEmailAddressChanged.Build(customerID, changedEmailAddress, changedConfirmationHash));
            emailAddress = changedEmailAddress;
            confirmationHash = changedConfirmationHash;
        }

        /**
     * Methods for WHEN
     */

        private void WHEN_RegisterCustomer() {
            var registerCustomer = global::RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            customerRegistered = Customer5.Register(registerCustomer);
            customerID = registerCustomer.CustomerId;
            confirmationHash = registerCustomer.ConfirmationHash;
        }

        private void WHEN_ConfirmEmailAddress_With(Hash confirmationHash) {
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
            try {
                recordedEvents = Customer5.ConfirmEmailAddress(eventStream, command);
            } catch (NullException e) {
                throw new XunitException(THelper.propertyIsNull("confirmationHash"));
            }
        }

        private void WHEN_ChangeEmailAddress_With(EmailAddress emailAddress) {
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, emailAddress.Value);
            try {
                recordedEvents = Customer5.ChangeEmailAddress(eventStream, command);
                changedConfirmationHash = command.ConfirmationHash;
            } catch (NullException e) {
                throw new XunitException(THelper.propertyIsNull("emailAddress"));
            }
        }

        /**
     * Methods for THEN
     */

        private void THEN_CustomerRegistered() {
            var method = "register";
            var eventName = "CustomerRegistered";
            Assert.NotNull(customerRegistered, THelper.eventIsNull(method, eventName));
            Assert.Equals(customerID, customerRegistered.CustomerID, THelper.propertyIsWrong(method, "customerID"));
            Assert.Equals(emailAddress, customerRegistered.EmailAddress, THelper.propertyIsWrong(method, "emailAddress"));
            Assert.Equals(confirmationHash, customerRegistered.ConfirmationHash, THelper.propertyIsWrong(method, "confirmationHash"));
            Assert.Equals(name, customerRegistered.name, THelper.propertyIsWrong(method, "name"));
        }

        private void THEN_EmailAddressConfirmed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmed";
            Assert.Equals(1, recordedEvents.Count(), THelper.noEventWasRecorded(method, eventName));
            Assert.NotNull(recordedEvents[0], THelper.eventIsNull(method, eventName));
            Assert.True(recordedEvents[0] is CustomerEmailAddressConfirmed, THelper.eventOfWrongTypeWasRecorded(method));
            var @event = (CustomerEmailAddressConfirmed) recordedEvents[0];
            Assert.True(Equals(customerID, @event.CustomerId), THelper.propertyIsWrong(method, "customerID"));
        }

        private void THEN_EmailAddressConfirmationFailed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmationFailed";
            Assert.Equals(1, recordedEvents.Count(), THelper.noEventWasRecorded(method, eventName));
            Assert.NotNull(recordedEvents[0], THelper.eventIsNull(method, eventName));
            Assert.Equals(CustomerEmailAddressConfirmationFailed.class, recordedEvents[0].getClass(), THelper.eventOfWrongTypeWasRecorded(method));
            var event = (CustomerEmailAddressConfirmationFailed) recordedEvents[0];
            Assert.Equals(customerID, event.customerID, THelper.propertyIsWrong(method, "customerID"));
        }

        private void THEN_EmailAddressChanged() {
            var method = "changeEmailAddress";
            var eventName = "CustomerEmailAddressChanged";
            Assert.Equals(1, recordedEvents.Count(), THelper.noEventWasRecorded(method, eventName));
            Assert.NotNull(recordedEvents[0], THelper.eventIsNull(method, eventName));
            Assert.Equals(CustomerEmailAddressChanged.class, recordedEvents[0].getClass(), THelper.eventOfWrongTypeWasRecorded(method));
            var event = (CustomerEmailAddressChanged) recordedEvents[0];
            Assert.Equals(customerID, event.customerID, THelper.propertyIsWrong(method, "customerID"));
            Assert.Equals(changedEmailAddress, event.emailAddress, THelper.propertyIsWrong(method, "emailAddress"));
            Assert.Equals(changedConfirmationHash, event.confirmationHash, THelper.propertyIsWrong(method, "confirmationHash"));
        }

        private void THEN_NothingShouldHappen() {
            Assert.Equals(0, recordedEvents.Count(),
                THelper.noEventShouldHaveBeenRecorded(THelper.typeOfFirst(recordedEvents)));
        }
    }
}
