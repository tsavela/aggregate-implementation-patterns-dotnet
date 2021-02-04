using System;
using System.Collections.Generic;
using Domain.Shared.Value;

public class ID : ValueObject
{
    public string Value { get; }

    private ID(string value)
    {
        Value = value;
    }

    public static ID Generate()
    {
        return new ID(Guid.NewGuid().ToString());
    }

    public static ID Build(string id)
    {
        return new ID(id);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
