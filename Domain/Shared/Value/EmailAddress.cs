using System.Collections.Generic;

namespace Domain.Shared.Value
{
    public class EmailAddress : ValueObject {
        public string Value { get; }

        private EmailAddress(string value) {
            Value = value;
        }

        public static EmailAddress Build(string emailAddress) {
            return new EmailAddress(emailAddress);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
