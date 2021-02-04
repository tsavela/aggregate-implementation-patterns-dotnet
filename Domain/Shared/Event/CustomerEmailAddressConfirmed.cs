namespace Domain.Shared.Event
{
    public class CustomerEmailAddressConfirmed : IEvent {
        public ID CustomerId { get; }

        private CustomerEmailAddressConfirmed(ID customerID)
        {
            CustomerId = customerID;
        }

        public static CustomerEmailAddressConfirmed Build(ID customerID) {
            return new CustomerEmailAddressConfirmed(customerID);
        }
    }
}
