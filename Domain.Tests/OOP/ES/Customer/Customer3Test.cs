
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.OOP.ES.Customer;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace Domain.Tests.OOP.ES.Customer
{
    public class Customer3Test {
        private ID customerID;
        private EmailAddress emailAddress;
        private EmailAddress changedEmailAddress;
        private Hash confirmationHash;
        private Hash wrongConfirmationHash;
        private Hash changedConfirmationHash;
        private PersonName name;
        private CustomerRegistered customerRegistered;
        private List<IEvent> recordedEvents;
        private Customer3 registeredCustomer;

        public Customer3Test()
        {
            customerID = ID.Generate();
            emailAddress = EmailAddress.Build("john@doe.com");
            changedEmailAddress = EmailAddress.Build("john+changed@doe.com");
            confirmationHash = Hash.Generate();
            wrongConfirmationHash = Hash.Generate();
            changedConfirmationHash = Hash.Generate();
            name = PersonName.Build("John", "Doe");
        }

        [Fact]
        void registerCustomer() {
            WHEN_RegisterCustomer();
            THEN_CustomerRegistered();
        }

        [Fact]
        void confirmEmailAddress() {
            GIVEN_CustomerRegistered();
            WHEN_ConfirmEmailAddress_With(confirmationHash);
            THEN_EmailAddressConfirmed();
        }

        [Fact]
        void confirmEmailAddress_withWrongConfirmationHash() {
            GIVEN_CustomerRegistered();
            WHEN_ConfirmEmailAddress_With(wrongConfirmationHash);
            THEN_EmailAddressConfirmationFailed();
        }

        [Fact]
        void confirmEmailAddress_whenItWasAlreadyConfirmed() {
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasConfirmed();
            WHEN_ConfirmEmailAddress_With(confirmationHash);
            THEN_NothingShouldHappen();
        }

        [Fact]
        void confirmEmailAddress_withWrongConfirmationHash_whenItWasAlreadyConfirmed() {
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasConfirmed();
            WHEN_ConfirmEmailAddress_With(wrongConfirmationHash);
            THEN_EmailAddressConfirmationFailed();
        }

        [Fact]
        void changeEmailAddress() {
            GIVEN_CustomerRegistered();
            WHEN_ChangeEmailAddress_With(changedEmailAddress);
            THEN_EmailAddressChanged();
        }

        [Fact]
        void changeEmailAddress_withUnchangedEmailAddress() {
            GIVEN_CustomerRegistered();
            WHEN_ChangeEmailAddress_With(emailAddress);
            THEN_NothingShouldHappen();
        }

        [Fact]
        void changeEmailAddress_whenItWasAlreadyChanged() {
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasChanged();
            WHEN_ChangeEmailAddress_With(changedEmailAddress);
            THEN_NothingShouldHappen();
        }

        [Fact]
        void confirmEmailAddress_whenItWasPreviouslyConfirmedAndThenChanged() {
            GIVEN_CustomerRegistered();
            __and_EmailAddressWasConfirmed();
            __and_EmailAddressWasChanged();
            WHEN_ConfirmEmailAddress_With(changedConfirmationHash);
            THEN_EmailAddressConfirmed();
        }

        /**
     * Methods for GIVEN
     */

        private void GIVEN_CustomerRegistered()
        {
            var events = new List<IEvent> {CustomerRegistered.Build(customerID, emailAddress, confirmationHash, name)};
            registeredCustomer = Customer3.Reconstitute(events);
        }

        private void __and_EmailAddressWasConfirmed() {
            registeredCustomer.Apply(
                CustomerEmailAddressConfirmed.Build(customerID)
            );
        }

        private void __and_EmailAddressWasChanged() {
            registeredCustomer.Apply(
                CustomerEmailAddressChanged.Build(customerID, changedEmailAddress, changedConfirmationHash)
            );
        }

        /**
     * Methods for WHEN
     */

        private void WHEN_RegisterCustomer()
        {
            var registerCustomer = RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            customerRegistered = Customer3.Register(registerCustomer);
            customerID = registerCustomer.CustomerId;
            confirmationHash = registerCustomer.ConfirmationHash;
        }

        private void WHEN_ConfirmEmailAddress_With(Hash confirmationHash) {
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
            try {
                recordedEvents = registeredCustomer.ConfirmEmailAddress(command);
            } catch (NullReferenceException e) {
                throw new XunitException(THelper.propertyIsNull("confirmationHash"));
            }
        }

        private void WHEN_ChangeEmailAddress_With(EmailAddress emailAddress) {
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, emailAddress.Value);
            try {
                recordedEvents = registeredCustomer.ChangeEmailAddress(command);
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
            recordedEvents.Count().Should().Be(1);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.True(@event is CustomerEmailAddressConfirmed);
            ((CustomerEmailAddressConfirmed) @event).CustomerId.Should().Be(customerID);
        }

        private void THEN_EmailAddressConfirmationFailed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmationFailed";
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.Equal(customerID, ((CustomerEmailAddressConfirmationFailed) @event).CustomerId);
            Assert.Equal(customerID, ((CustomerEmailAddressConfirmationFailed) @event).CustomerId);
        }

        private void THEN_EmailAddressChanged() {
            var method = "changeEmailAddress";
            var eventName = "CustomerEmailAddressChanged";
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.True(@event is CustomerEmailAddressChanged);
            Assert.Equal(customerID, ((CustomerEmailAddressChanged) @event).CustomerId);
            Assert.Equal(changedEmailAddress, ((CustomerEmailAddressChanged) @event).EmailAddress);
        }

        private void THEN_NothingShouldHappen() {
            Assert.Empty(recordedEvents);
        }
    }
}
