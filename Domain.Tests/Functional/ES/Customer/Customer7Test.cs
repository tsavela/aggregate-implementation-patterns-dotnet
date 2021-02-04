using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Functional.ES.Customer;
using Domain.Shared.Command;
using Domain.Shared.Event;
using Domain.Shared.Value;
using Xunit;
using Xunit.Sdk;

namespace Domain.Tests.Functional.ES.Customer
{
    public class Customer7Test {
        private ID customerID;
        private EmailAddress emailAddress;
        private EmailAddress changedEmailAddress;
        private Hash confirmationHash;
        private Hash wrongConfirmationHash;
        private Hash changedConfirmationHash;
        private PersonName name;
        private CustomerState currentState;
        private CustomerRegistered customerRegistered;
        private List<IEvent> recordedEvents;

        public Customer7Test() {
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
            WHEN_ConfirmEmailAddress_With(confirmationHash);
            THEN_EmailAddressConfirmed();
        }

        [Fact]
        void confirmEmailAddress_withWrongConfirmationHash_whenItWasAlreadyConfirmed() {
            GIVEN_CustomerRegistered();
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

        private void GIVEN_CustomerRegistered() {
            currentState = CustomerState.Reconstitute(
                new List<IEvent> {
                    CustomerRegistered.Build(customerID, emailAddress, confirmationHash, name)
                }
            );
        }

        private void __and_EmailAddressWasConfirmed() {
            currentState.Apply(
                new List<IEvent> {
                    CustomerEmailAddressConfirmed.Build(customerID)
                }
            );
        }

        private void __and_EmailAddressWasChanged() {
            currentState.Apply(
                new List<IEvent> {
                    CustomerEmailAddressChanged.Build(customerID, changedEmailAddress, changedConfirmationHash)
                }
            );
        }

        /**
     * Methods for WHEN
     */

        private void WHEN_RegisterCustomer() {
            var registerCustomer = RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            customerRegistered = Customer7.Register(registerCustomer);
            customerID = registerCustomer.CustomerId;
            confirmationHash = registerCustomer.ConfirmationHash;
        }

        private void WHEN_ConfirmEmailAddress_With(Hash confirmationHash) {
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
            try {
                recordedEvents = Customer7.ConfirmEmailAddress(currentState, command);
            } catch (NullReferenceException e) {
                throw new XunitException(THelper.propertyIsNull("confirmationHash"));
            }
        }

        private void WHEN_ChangeEmailAddress_With(EmailAddress emailAddress) {
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, emailAddress.Value);
            try {
                recordedEvents = Customer7.ChangeEmailAddress(currentState, command);
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
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.True(@event is CustomerEmailAddressConfirmed);
            var typedEvent = (CustomerEmailAddressConfirmed) @event;
            Assert.Equal(customerID, typedEvent.CustomerId);
        }

        private void THEN_EmailAddressConfirmationFailed() {
            var method = "confirmEmailAddress";
            var eventName = "CustomerEmailAddressConfirmationFailed";
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.True(@event is CustomerEmailAddressConfirmationFailed);
            var typedEvent = (CustomerEmailAddressConfirmationFailed) @event;
            Assert.Equal(customerID, typedEvent.CustomerId);
        }

        private void THEN_EmailAddressChanged() {
            var method = "changeEmailAddress";
            var eventName = "CustomerEmailAddressChanged";
            Assert.Single(recordedEvents);
            var @event = recordedEvents[0];
            Assert.NotNull(@event);
            Assert.True(@event is CustomerEmailAddressChanged);
            var typedEvent = (CustomerEmailAddressChanged) @event;
            Assert.Equal(customerID, typedEvent.CustomerId);
            Assert.Equal(changedEmailAddress, typedEvent.EmailAddress);
            Assert.Equal(changedConfirmationHash, typedEvent.ConfirmationHash);
        }

        private void THEN_NothingShouldHappen() {
            Assert.Empty(recordedEvents);
        }
    }
}
