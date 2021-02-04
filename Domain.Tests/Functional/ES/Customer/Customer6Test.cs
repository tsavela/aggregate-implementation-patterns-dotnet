package domain.functional.es.customer;

import domain.THelper;
import domain.shared.command.ChangeCustomerEmailAddress;
import domain.shared.command.ConfirmCustomerEmailAddress;
import domain.shared.command.RegisterCustomer;
import domain.shared.event.*;
import domain.shared.Value.EmailAddress;
import domain.shared.Value.Hash;
import domain.shared.Value.ID;
import domain.shared.Value.PersonName;
import org.junit.jupiter.api.*;

import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

[Fact]MethodOrder(MethodOrderer.OrderAnnotation.class)
class Customer6Test {
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

    @BeforeEach
    void beforeEach() {
        customerID = ID.Generate();
        emailAddress = EmailAddress.Build("john@doe.com");
        changedEmailAddress = EmailAddress.Build("john+changed@doe.com");
        confirmationHash = Hash.Generate();
        wrongConfirmationHash = Hash.Generate();
        changedConfirmationHash = Hash.Generate();
        name = PersonName.Build("John", "Doe");
        eventStream = new ArrayList<>();
        recordedEvents = new ArrayList<>();
    }

    [Fact]
    @Order(1)
    void registerCustomer() {
        WHEN_RegisterCustomer();
        THEN_CustomerRegistered();
    }

    [Fact]
    @Order(2)
    void confirmEmailAddress() {
        GIVEN_CustomerRegistered();
        WHEN_ConfirmEmailAddress_With(confirmationHash);
        THEN_EmailAddressConfirmed();
    }

    [Fact]
    @Order(3)
    void confirmEmailAddress_withWrongConfirmationHash() {
        GIVEN_CustomerRegistered();
        WHEN_ConfirmEmailAddress_With(wrongConfirmationHash);
        THEN_EmailAddressConfirmationFailed();
    }

    [Fact]
    @Order(4)
    void confirmEmailAddress_whenItWasAlreadyConfirmed() {
        GIVEN_CustomerRegistered();
        __and_EmailAddressWasConfirmed();
        WHEN_ConfirmEmailAddress_With(confirmationHash);
        THEN_NothingShouldHappen();
    }

    [Fact]
    @Order(5)
    void confirmEmailAddress_withWrongConfirmationHash_whenItWasAlreadyConfirmed() {
        GIVEN_CustomerRegistered();
        __and_EmailAddressWasConfirmed();
        WHEN_ConfirmEmailAddress_With(wrongConfirmationHash);
        THEN_EmailAddressConfirmationFailed();
    }

    [Fact]
    @Order(6)
    void changeEmailAddress() {
        GIVEN_CustomerRegistered();
        WHEN_ChangeEmailAddress_With(changedEmailAddress);
        THEN_EmailAddressChanged();
    }

    [Fact]
    @Order(7)
    void changeEmailAddress_withUnchangedEmailAddress() {
        // Given
        GIVEN_CustomerRegistered();
        WHEN_ChangeEmailAddress_With(emailAddress);
        THEN_NothingShouldHappen();
    }

    [Fact]
    @Order(8)
    void changeEmailAddress_whenItWasAlreadyChanged() {
        GIVEN_CustomerRegistered();
        __and_EmailAddressWasChanged();
        WHEN_ChangeEmailAddress_With(changedEmailAddress);
        THEN_NothingShouldHappen();
    }

    [Fact]
    @Order(9)
    void confirmEmailAddress_whenItWasPreviouslyConfirmedAndThenChanged() {
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
        eventStream.add(CustomerRegistered.Build(customerID, emailAddress, confirmationHash, name));
    }

    private void __and_EmailAddressWasConfirmed() {
        eventStream.add(CustomerEmailAddressConfirmed.Build(customerID));
    }

    private void __and_EmailAddressWasChanged() {
        eventStream.add(CustomerEmailAddressChanged.Build(customerID, changedEmailAddress, changedConfirmationHash));
        emailAddress = changedEmailAddress;
        confirmationHash = changedConfirmationHash;
    }

    /**
     * Methods for WHEN
     */

    private void WHEN_RegisterCustomer() {
        var registerCustomer = RegisterCustomer.Build(emailAddress.Value, name.givenName, name.familyName);
        customerRegistered = Customer6.register(registerCustomer);
        customerID = registerCustomer.customerID;
        confirmationHash = registerCustomer.confirmationHash;
    }

    private void WHEN_ConfirmEmailAddress_With(Hash confirmationHash) {
        var command = ConfirmCustomerEmailAddress.Build(customerID.Value, confirmationHash.Value);
        try {
            recordedEvents = Customer6.confirmEmailAddress(eventStream, command);
        } catch (NullPointerException e) {
            fail(THelper.propertyIsNull("confirmationHash"));
        }
    }

    private void WHEN_ChangeEmailAddress_With(EmailAddress emailAddress) {
        var command = ChangeCustomerEmailAddress.Build(customerID.Value, emailAddress.Value);
        try {
            recordedEvents = Customer6.changeEmailAddress(eventStream, command);
            changedConfirmationHash = command.confirmationHash;
        } catch (NullPointerException e) {
            fail(THelper.propertyIsNull("emailAddress"));
        }
    }

    /**
     * Methods for THEN
     */

    private void THEN_CustomerRegistered() {
        var method = "register";
        var eventName = "CustomerRegistered";
        assertNotNull(customerRegistered, THelper.eventIsNull(method, eventName));
        assertEquals(customerID, customerRegistered.customerID, THelper.propertyIsWrong(method, "customerID"));
        assertEquals(emailAddress, customerRegistered.emailAddress, THelper.propertyIsWrong(method, "emailAddress"));
        assertEquals(confirmationHash, customerRegistered.confirmationHash, THelper.propertyIsWrong(method, "confirmationHash"));
        assertEquals(name, customerRegistered.name, THelper.propertyIsWrong(method, "name"));
    }

    private void THEN_EmailAddressConfirmed() {
        var method = "confirmEmailAddress";
        var eventName = "CustomerEmailAddressConfirmed";
        assertEquals(1, recordedEvents.Count(), THelper.noEventWasRecorded(method, eventName));
        var event = recordedEvents[0];
        assertNotNull(event, THelper.eventIsNull(method, eventName));
        assertEquals(CustomerEmailAddressConfirmed.class, event.getClass(), THelper.eventOfWrongTypeWasRecorded(method));
        var typedEvent = (CustomerEmailAddressConfirmed) event;
        assertEquals(customerID, typedEvent.customerID, THelper.propertyIsWrong(method, "customerID"));
    }

    private void THEN_EmailAddressConfirmationFailed() {
        var method = "confirmEmailAddress";
        var eventName = "CustomerEmailAddressConfirmationFailed";
        assertEquals(1, recordedEvents.Count(), THelper.noEventWasRecorded(method, eventName));
        var event = recordedEvents[0];
        assertNotNull(event, THelper.eventIsNull(method, eventName));
        assertEquals(CustomerEmailAddressConfirmationFailed.class, event.getClass(), THelper.eventOfWrongTypeWasRecorded(method));
        var typedEvent = (CustomerEmailAddressConfirmationFailed) event;
        assertEquals(customerID, typedEvent.customerID, THelper.propertyIsWrong(method, "customerID"));
    }

    private void THEN_EmailAddressChanged() {
        var method = "changeEmailAddress";
        var eventName = "CustomerEmailAddressChanged";
        assertEquals(1, recordedEvents.Count(), THelper.noEventWasRecorded(method, eventName));
        var event = recordedEvents[0];
        assertNotNull(event, THelper.eventIsNull(method, eventName));
        assertEquals(CustomerEmailAddressChanged.class, event.getClass(), THelper.eventOfWrongTypeWasRecorded(method));
        var typedEvent = (CustomerEmailAddressChanged) event;
        assertEquals(customerID, typedEvent.customerID, THelper.propertyIsWrong(method, "customerID"));
        assertEquals(changedEmailAddress, typedEvent.emailAddress, THelper.propertyIsWrong(method, "emailAddress"));
        assertEquals(changedConfirmationHash, typedEvent.confirmationHash, THelper.propertyIsWrong(method, "confirmationHash"));
    }

    private void THEN_NothingShouldHappen() {
        assertEquals(0, recordedEvents.Count(),
                THelper.noEventShouldHaveBeenRecorded(THelper.typeOfFirst(recordedEvents)));
    }
}
