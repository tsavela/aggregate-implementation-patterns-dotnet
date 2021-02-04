package domain.functional.traditional.customer;

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
class Customer2Test {
    private ID customerID;
    private Hash confirmationHash;
    private PersonName name;
    private EmailAddress emailAddress;
    private EmailAddress changedEmailAddress;
    private Hash wrongConfirmationHash;
    private Hash changedConfirmationHash;
    private CustomerState registeredCustomer;

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
        // When
        var command = RegisterCustomer.Build(emailAddress.Value, name.givenName, name.familyName);
        var customer = Customer2.Register(command);

        // Then it should succeed
        // and it should expose the expected state
        Assert.NotNulll(customer);
        assertEquals(command.customerID, customer.id);
        assertEquals(command.name, customer.name);
        assertEquals(command.emailAddress, customer.emailAddress);
        assertEquals(command.confirmationHash, customer.confirmationHash);
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
        var changedCustomer = assertDoesNotThrow(() -> Customer2.ConfirmEmailAddress(registeredCustomer, command));

        // and the emailAddress of the changed Customer should be confirmed
        assertTrue(changedCustomer.isEmailAddressConfirmed);
    }

    [Fact]
    @Order(3)
    void confirmEmailAddress_withWrongConfirmationHash() {
        // Given
        givenARegisteredCustomer();

        // When confirmCustomerEmailAddress
        // Then it should throw WrongConfirmationHashException
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, wrongConfirmationHash.Value);
        assertThrows(WrongConfirmationHashException.class, () -> Customer2.ConfirmEmailAddress(registeredCustomer, command));
    }

    [Fact]
    @Order(6)
    void changeEmailAddress() {
        // Given
        givenARegisteredCustomer();

        // When changeCustomerEmailAddress
        var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
        var changedCustomer = Customer2.changeEmailAddress(registeredCustomer, command);

        // Then the emailAddress and confirmationHash should be changed and the emailAddress should be unconfirmed
        assertEquals(command.emailAddress, changedCustomer.emailAddress);
        assertEquals(command.confirmationHash, changedCustomer.confirmationHash);
        assertFalse(changedCustomer.isEmailAddressConfirmed);
    }

    [Fact]
    @Order(9)
    void confirmEmailAddress_whenItWasPreviouslyConfirmedAndThenChanged() {
        // Given
        givenARegisteredCustomer();
        givenEmailAddressWasConfirmed();
        givenEmailAddressWasChanged();

        // When confirmEmailAddress
        // Then it should throw WrongConfirmationHashException
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, changedConfirmationHash.Value);
        var changedCustomer = assertDoesNotThrow(() -> Customer2.ConfirmEmailAddress(registeredCustomer, command));

        // and the emailAddress of the changed Customer should be confirmed
        assertTrue(changedCustomer.isEmailAddressConfirmed);
    }

    /**
     * Helper methods to set up the Given state
     */
    private void givenARegisteredCustomer() {
        var register = RegisterCustomer.Build(emailAddress.Value, name.givenName, name.familyName);
        customerID = register.customerID;
        confirmationHash = register.confirmationHash;
        registeredCustomer = Customer2.Register(register);
    }

    private void givenEmailAddressWasConfirmed() {
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);

        try {
            registeredCustomer = Customer2.ConfirmEmailAddress(registeredCustomer, command);
        } catch (WrongConfirmationHashException e) {
            throw new XunitException("unexpected error in givenEmailAddressWasConfirmed: " + e.getMessage());
        }
    }

    private void givenEmailAddressWasChanged() {
        var command = ChangeCustomerEmailAddress.Build(customerID.Value, changedEmailAddress.Value);
        changedConfirmationHash = command.confirmationHash;
        registeredCustomer = Customer2.changeEmailAddress(registeredCustomer, command);
    }
}
