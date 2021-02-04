using System;
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
            var registerCustomer = Shared.Command.RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            customerRegistered = Customer5.Register(registerCustomer);
            customerID = registerCustomer.CustomerId;
            confirmationHash = registerCustomer.ConfirmationHash;
        }

        private void WHEN_ConfirmEmailAddress_With(Hash confirmationHash) {
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
            try {
                recordedEvents = Customer5.ConfirmEmailAddress(eventStream, command);
            } catch (NullReferenceException e) {
                throw new XunitException(THelper.propertyIsNull("confirmationHash"));
            }
        }

        private void WHEN_ChangeEmailAddress_With(EmailAddress emailAddress) {
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, emailAddress.Value);
            try {
                recordedEvents = Customer5.ChangeEmailAddress(eventStream, command);
                changedConfirmationHash = command.ConfirmationHash;
            } catch (NullReferenceException e) {
                throw new XunitException(THelper.propertyIsNull("emailAddress"));
            }
        }

        /**
     * Methods for THEN
     */

        private void THEN_CustomerRegistered() {
            var method = "register";
            var eventName = "CustomerRegistered";
            Assert.NotNull(customerRegistered);
            Assert.Equal(customerID, customerRegistered.CustomerId);
            Assert.Equal(emailAddress, customerRegistered.EmailAddress);
            Assert.Equal(confirmationHash, customerRegistered.ConfirmationHash);
            Assert.Equal(name, customerRegistered.Name);
        }

        private void THEN_EmailAddressConfirmed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmed";
            Assert.Single(recordedEvents);
            Assert.NotNull(recordedEvents[0]);
            Assert.True(recordedEvents[0] is CustomerEmailAddressConfirmed, THelper.eventOfWrongTypeWasRecorded(method));
            var @event = (CustomerEmailAddressConfirmed) recordedEvents[0];
            Assert.True(Equals(customerID, @event.CustomerId), THelper.propertyIsWrong(method, "customerID"));
        }

        private void THEN_EmailAddressConfirmationFailed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmationFailed";
            Assert.Single(recordedEvents);
            Assert.NotNull(recordedEvents[0]);
            Assert.True(recordedEvents[0] is CustomerEmailAddressConfirmationFailed);
            Assert.Equal(customerID, ((CustomerEmailAddressConfirmationFailed) recordedEvents[0]).CustomerId);
        }

        private void THEN_EmailAddressChanged() {
            var method = "changeEmailAddress";
            var eventName = "CustomerEmailAddressChanged";
            Assert.Single(recordedEvents);
            Assert.NotNull(recordedEvents[0]);
            Assert.True(recordedEvents[0] is CustomerEmailAddressChanged);
            var @event = ((CustomerEmailAddressChanged) recordedEvents[0]);
            Assert.Equal(customerID, @event.CustomerId);
            Assert.Equal(changedEmailAddress, @event.EmailAddress);
            Assert.Equal(changedConfirmationHash, @event.ConfirmationHash);
        }

        private void THEN_NothingShouldHappen() {
            Assert.Empty(recordedEvents);
        }
    }
}
