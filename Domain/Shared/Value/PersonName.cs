using System.Collections.Generic;

namespace Domain.Shared.Value
{
    public class PersonName : ValueObject {
        public string GivenName { get; }
        public string FamilyName { get; }

        private PersonName(string givenName, string familyName)
        {
            GivenName = givenName;
            FamilyName = familyName;
        }

        public static PersonName Build(string givenName, string familyName) {
            return new PersonName(givenName, familyName);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return GivenName;
            yield return FamilyName;
        }
    }
}
