using System;
using System.Collections.Generic;
using Domain.OOP.ES.Customer;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;
using Xunit;
using Xunit.Sdk;

namespace Domain.Tests.OOP.ES.Customer
{
    public class Customer4Test {
        private ID customerID;
        private EmailAddress emailAddress;
        private EmailAddress changedEmailAddress;
        private Hash confirmationHash;
        private Hash wrongConfirmationHash;
        private Hash changedConfirmationHash;
        private PersonName name;
        private Customer4 registeredCustomer;

        public Customer4Test() {
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
            // Given
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

        private void GIVEN_CustomerRegistered() {
            registeredCustomer = Customer4.Reconstitute(
                new List<IEvent> {
                    CustomerRegistered.Build(customerID, emailAddress, confirmationHash, name)
                }
            );
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

        private void WHEN_RegisterCustomer() {
            var registerCustomer = RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            registeredCustomer = Customer4.Register(registerCustomer);
            customerID = registerCustomer.CustomerId;
            confirmationHash = registerCustomer.ConfirmationHash;
        }

        private void WHEN_ConfirmEmailAddress_With(Hash confirmationHash) {
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
            try {
                registeredCustomer.ConfirmEmailAddress(command);
            } catch (NullReferenceException e) {
                throw new XunitException(THelper.propertyIsNull("confirmationHash"));
            }
        }

        private void WHEN_ChangeEmailAddress_With(EmailAddress emailAddress) {
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, emailAddress.Value);
            try {
                registeredCustomer.ChangeEmailAddress(command);
            } catch (NullReferenceException e) {
                throw new XunitException(THelper.propertyIsNull("emailAddress"));
            }
        }

        /**
     * Methods for THEN
     */

        void THEN_CustomerRegistered() {
            var method = "register";
            var eventName = "CustomerRegistered";
            var recordedEvents = registeredCustomer.GetRecordedEvents();
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.IsType<CustomerRegistered>(@event);
            Assert.Equal(customerID, ((CustomerRegistered) @event).CustomerId);
            Assert.Equal(emailAddress, ((CustomerRegistered) @event).EmailAddress);
            Assert.Equal(confirmationHash, ((CustomerRegistered) @event).ConfirmationHash);
            Assert.Equal(name, ((CustomerRegistered) @event).Name);
        }

        void THEN_EmailAddressConfirmed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmed";
            var recordedEvents = registeredCustomer.GetRecordedEvents();
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.IsType<CustomerEmailAddressConfirmed>(@event);
            Assert.Equal(customerID, ((CustomerEmailAddressConfirmed) @event).CustomerId);
        }

        void THEN_EmailAddressConfirmationFailed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmationFailed";
            var recordedEvents = registeredCustomer.GetRecordedEvents();
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.IsType<CustomerEmailAddressConfirmationFailed>(@event);
            Assert.Equal(customerID, ((CustomerEmailAddressConfirmationFailed) @event).CustomerId);
        }

        private void THEN_EmailAddressChanged() {
            var method = "changeEmailAddress";
            var eventName = "CustomerEmailAddressChanged";
            var recordedEvents = registeredCustomer.GetRecordedEvents();
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.IsType<CustomerEmailAddressChanged>(@event);
            Assert.Equal(customerID, ((CustomerEmailAddressChanged) @event).CustomerId);
            Assert.Equal(changedEmailAddress, ((CustomerEmailAddressChanged) @event).EmailAddress);
        }

        void THEN_NothingShouldHappen() {
            Assert.Empty(registeredCustomer.GetRecordedEvents());
        }
    }
}
