using System;
using System.Collections.Generic;

namespace Domain.Shared.Value
{
    public class Hash : ValueObject {
        public string Value { get; }

        private Hash(string value)
        {
            Value = value;
        }

        public static Hash Generate() {
            return new Hash(Guid.NewGuid().ToString());
        }

        public static Hash Build(string hash) {
            return new Hash(hash);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
