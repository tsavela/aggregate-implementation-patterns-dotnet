
using Domain.Functional.Traditional.Customer;
using Domain.Shared.Command;
using Domain.Shared.Exception;
using Domain.Shared.Value;
using Xunit;
using Xunit.Sdk;

namespace Domain.Tests.Functional.Traditional.Customer
{
    public class Customer2Test {
        private ID customerID;
        private Hash confirmationHash;
        private PersonName name;
        private EmailAddress emailAddress;
        private EmailAddress changedEmailAddress;
        private Hash wrongConfirmationHash;
        private Hash changedConfirmationHash;
        private CustomerState registeredCustomer;

        public Customer2Test() {
            emailAddress = EmailAddress.Build("john@doe.com");
            changedEmailAddress = EmailAddress.Build("john+changed@doe.com");
            wrongConfirmationHash = Hash.Generate();
            changedConfirmationHash = Hash.Generate();
            name = PersonName.Build("John", "Doe");
        }

        [Fact]
        void registerCustomer() {
            // When
            var command = RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            var customer = Customer2.Register(command);

            // Then it should succeed
            // and it should expose the expected state
            Assert.NotNull(customer);
            Assert.Equal(command.CustomerId, customer.CustomerId);
            Assert.Equal(command.Name, customer.Name);
            Assert.Equal(command.EmailAddress, customer.EmailAddress);
            Assert.Equal(command.ConfirmationHash, customer.ConfirmationHash);
            Assert.False(customer.IsEmailAddressConfirmed);
        }

        [Fact]
        void confirmEmailAddress() {
            // Given
            givenARegisteredCustomer();

            // When confirmCustomerEmailAddress
            // Then it should succeed
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
            var changedCustomer = Customer2.ConfirmEmailAddress(registeredCustomer, command);

            // and the emailAddress of the changed Customer should be confirmed
            Assert.True(changedCustomer.IsEmailAddressConfirmed);
        }

        [Fact]
        void confirmEmailAddress_withWrongConfirmationHash() {
            // Given
            givenARegisteredCustomer();

            // When confirmCustomerEmailAddress
            // Then it should throw WrongConfirmationHashException
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, wrongConfirmationHash.Value);
            Assert.Throws<WrongConfirmationHashException>(() => Customer2.ConfirmEmailAddress(registeredCustomer, command));
        }

        [Fact]
        void changeEmailAddress() {
            // Given
            givenARegisteredCustomer();

            // When changeCustomerEmailAddress
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
            var changedCustomer = Customer2.ChangeEmailAddress(registeredCustomer, command);

            // Then the emailAddress and confirmationHash should be changed and the emailAddress should be unconfirmed
            Assert.Equal(command.EmailAddress, changedCustomer.EmailAddress);
            Assert.Equal(command.ConfirmationHash, changedCustomer.ConfirmationHash);
            Assert.False(changedCustomer.IsEmailAddressConfirmed);
        }

        [Fact]
        void confirmEmailAddress_whenItWasPreviouslyConfirmedAndThenChanged() {
            // Given
            givenARegisteredCustomer();
            givenEmailAddressWasConfirmed();
            givenEmailAddressWasChanged();

            // When confirmEmailAddress
            // Then it should throw WrongConfirmationHashException
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, changedConfirmationHash.Value);
            var changedCustomer = Customer2.ConfirmEmailAddress(registeredCustomer, command);

            // and the emailAddress of the changed Customer should be confirmed
            Assert.True(changedCustomer.IsEmailAddressConfirmed);
        }

        /**
     * Helper methods to set up the Given state
     */
        private void givenARegisteredCustomer() {
            var register = RegisterCustomer.Build(emailAddress.Value, name.GivenName, name.FamilyName);
            customerID = register.CustomerId;
            confirmationHash = register.ConfirmationHash;
            registeredCustomer = Customer2.Register(register);
        }

        private void givenEmailAddressWasConfirmed() {
            var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);

            try {
                registeredCustomer = Customer2.ConfirmEmailAddress(registeredCustomer, command);
            } catch (WrongConfirmationHashException e) {
                throw new XunitException("unexpected error in givenEmailAddressWasConfirmed: " + e.Message);
            }
        }

        private void givenEmailAddressWasChanged() {
            var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
            changedConfirmationHash = command.ConfirmationHash;
            registeredCustomer = Customer2.ChangeEmailAddress(registeredCustomer, command);
        }
    }
}
