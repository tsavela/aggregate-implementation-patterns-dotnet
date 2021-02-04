using Domain.OOP.Traditional.Customer;
using Domain.Shared.Command;
using Domain.Shared.Exception;
using Domain.Shared.Value;
using Xunit;
using Xunit.Sdk;

namespace Domain.Tests.OOP.Traditional.Customer
{
    public class Customer1Test {
        private ID customerID;
        private Hash confirmationHash;
        private PersonName name;
        private EmailAddress emailAddress;
        private EmailAddress changedEmailAddress;
        private Hash wrongConfirmationHash;
        private Hash changedConfirmationHash;
        private Customer1 registeredCustomer;

        public Customer1Test() {
            emailAddress = EmailAddress.Build("john@doe.com");
            changedEmailAddress = EmailAddress.Build("john+changed@doe.com");
            wrongConfirmationHash = Hash.Generate();
            changedConfirmationHash = Hash.Generate();
            name = PersonName.Build("John", "Doe");
        }

        [Fact]
        void registerCustomer() {
            // When registerCustomer
            var command = RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            var customer = Customer1.Register(command);

            // Then it should succeed
            // and should have the expected state
            Assert.NotNull(customer);
            Assert.Equal(customer.CustomerId, command.CustomerId);
            Assert.Equal(customer.Name, command.Name);
            Assert.Equal(customer.EmailAddress, command.EmailAddress);
            Assert.Equal(customer.ConfirmationHash, command.ConfirmationHash);
            Assert.False(customer.IsEmailAddressConfirmed);
        }

        [Fact]
        void confirmEmailAddress() {
            // Given
            givenARegisteredCustomer();

            // When confirmCustomerEmailAddress
            // Then it should succeed
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
            registeredCustomer.ConfirmEmailAddress(command);

            // and the emailAddress should be confirmed
            Assert.True(registeredCustomer.IsEmailAddressConfirmed);
        }

        [Fact]
        void confirmEmailAddress_withWrongConfirmationHash() {
            // Given
            givenARegisteredCustomer();

            // When confirmCustomerEmailAddress
            // Then it should throw WrongConfirmationHashException
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, wrongConfirmationHash.Value);
            Assert.Throws<WrongConfirmationHashException>(() => registeredCustomer.ConfirmEmailAddress(command));

            // and the emailAddress should not be confirmed
            Assert.False(registeredCustomer.IsEmailAddressConfirmed);
        }

        [Fact]
        void changeEmailAddress() {
            // Given
            givenARegisteredCustomer();

            // When changeCustomerEmailAddress
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
            registeredCustomer.ChangeEmailAddress(command);

            // Then the emailAddress and confirmationHash should be changed and the emailAddress should be unconfirmed
            Assert.Equal(registeredCustomer.EmailAddress, command.EmailAddress);
            Assert.Equal(registeredCustomer.ConfirmationHash, command.ConfirmationHash);
            Assert.False(registeredCustomer.IsEmailAddressConfirmed);
        }

        [Fact]
        void confirmEmailAddress_whenItWasPreviouslyConfirmedAndThenChanged() {
            // Given
            givenARegisteredCustomer();
            givenEmailAddressWasConfirmed();
            givenEmailAddressWasChanged();

            // When confirmCustomerEmailAddress
            // Then it should succeed
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, changedConfirmationHash.Value);
            registeredCustomer.ConfirmEmailAddress(command);

            // and the emailAddress should be confirmed
            Assert.True(registeredCustomer.IsEmailAddressConfirmed);
        }

        /**
     * Helper methods to set up the Given state
     */
        private void givenARegisteredCustomer() {
            var register = RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            customerID = register.CustomerId;
            confirmationHash = register.ConfirmationHash;
            registeredCustomer = Customer1.Register(register);
        }

        private void givenEmailAddressWasConfirmed() {
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);

            try {
                registeredCustomer.ConfirmEmailAddress(command);
            } catch (WrongConfirmationHashException e) {
                throw new XunitException("unexpected error in givenEmailAddressWasConfirmed: " + e.Message);
            }
        }

        private void givenEmailAddressWasChanged() {
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
            changedConfirmationHash = command.ConfirmationHash;
            registeredCustomer.ChangeEmailAddress(command);
        }
    }
}
