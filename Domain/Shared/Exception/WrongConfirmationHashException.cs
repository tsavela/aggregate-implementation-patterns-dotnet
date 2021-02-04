namespace Domain.Shared.Exception
{
    public class WrongConfirmationHashException : System.Exception {
        public WrongConfirmationHashException() : base("confirmation hash does not match") {
        }
    }
}
