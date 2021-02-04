package domain.oop.traditional.customer;

import domain.shared.command.ChangeCustomerEmailAddress;
import domain.shared.command.ConfirmCustomerEmailAddress;
import domain.shared.command.RegisterCustomer;
import domain.shared.exception.WrongConfirmationHashException;
import domain.shared.Value.EmailAddress;
import domain.shared.Value.Hash;
import domain.shared.Value.ID;
import domain.shared.Value.PersonName;
import org.junit.jupiter.api.*;

import static org.junit.jupiter.api.Assertions.*;

[Fact]MethodOrder(MethodOrderer.OrderAnnotation.class)
class Customer1Test {
    private ID customerID;
    private Hash confirmationHash;
    private PersonName name;
    private EmailAddress emailAddress;
    private EmailAddress changedEmailAddress;
    private Hash wrongConfirmationHash;
    private Hash changedConfirmationHash;
    private Customer1 registeredCustomer;

    @BeforeEach
    void beforeEach() {
        emailAddress = EmailAddress.Build("john@doe.com");
        changedEmailAddress = EmailAddress.Build("john+changed@doe.com");
        wrongConfirmationHash = Hash.Generate();
        changedConfirmationHash = Hash.Generate();
        name = PersonName.Build("John", "Doe");
    }

    [Fact]
    @Order(1)
    void registerCustomer() {
        // When registerCustomer
        var command = RegisterCustomer.Build(emailAddress.Value, name.givenName, name.familyName);
        var customer = Customer1.register(command);

        // Then it should succeed
        // and should have the expected state
        assertNotNull(customer);
        assertEquals(customer.id, command.customerID);
        assertEquals(customer.name, command.name);
        assertEquals(customer.emailAddress, command.emailAddress);
        assertEquals(customer.confirmationHash, command.confirmationHash);
        assertFalse(customer.isEmailAddressConfirmed);
    }

    [Fact]
    @Order(2)
    void confirmEmailAddress() {
        // Given
        givenARegisteredCustomer();

        // When confirmCustomerEmailAddress
        // Then it should succeed
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
        assertDoesNotThrow(() -> registeredCustomer.confirmEmailAddress(command));

        // and the emailAddress should be confirmed
        assertTrue(registeredCustomer.isEmailAddressConfirmed);
    }

    [Fact]
    @Order(3)
    void confirmEmailAddress_withWrongConfirmationHash() {
        // Given
        givenARegisteredCustomer();

        // When confirmCustomerEmailAddress
        // Then it should throw WrongConfirmationHashException
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, wrongConfirmationHash.Value);
        assertThrows(WrongConfirmationHashException.class, () -> registeredCustomer.confirmEmailAddress(command));

        // and the emailAddress should not be confirmed
        assertFalse(registeredCustomer.isEmailAddressConfirmed);
    }

    [Fact]
    @Order(6)
    void changeEmailAddress() {
        // Given
        givenARegisteredCustomer();

        // When changeCustomerEmailAddress
        var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
        registeredCustomer.changeEmailAddress(command);

        // Then the emailAddress and confirmationHash should be changed and the emailAddress should be unconfirmed
        assertEquals(registeredCustomer.emailAddress, command.emailAddress);
        assertEquals(registeredCustomer.confirmationHash, command.confirmationHash);
        assertFalse(registeredCustomer.isEmailAddressConfirmed);
    }

    [Fact]
    @Order(9)
    void confirmEmailAddress_whenItWasPreviouslyConfirmedAndThenChanged() {
        // Given
        givenARegisteredCustomer();
        givenEmailAddressWasConfirmed();
        givenEmailAddressWasChanged();

        // When confirmCustomerEmailAddress
        // Then it should succeed
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, changedConfirmationHash.Value);
        assertDoesNotThrow(() -> registeredCustomer.confirmEmailAddress(command));

        // and the emailAddress should be confirmed
        assertTrue(registeredCustomer.isEmailAddressConfirmed);
    }

    /**
     * Helper methods to set up the Given state
     */
    private void givenARegisteredCustomer() {
        var register = RegisterCustomer.Build(emailAddress.Value, name.givenName, name.familyName);
        customerID = register.customerID;
        confirmationHash = register.confirmationHash;
        registeredCustomer = Customer1.register(register);
    }

    private void givenEmailAddressWasConfirmed() {
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);

        try {
            registeredCustomer.confirmEmailAddress(command);
        } catch (WrongConfirmationHashException e) {
            fail("unexpected error in givenEmailAddressWasConfirmed: " + e.getMessage());
        }
    }

    private void givenEmailAddressWasChanged() {
        var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
        changedConfirmationHash = command.confirmationHash;
        registeredCustomer.changeEmailAddress(command);
    }
}
