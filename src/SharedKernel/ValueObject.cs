namespace SharedKernel;

public abstract class ValueObject
{

    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        return ReferenceEquals(left, right) || left is not null && left.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is not ValueObject other || GetType() != obj.GetType())
        {
            return false;
        }

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(0, (hash, obj) =>
                HashCode.Combine(hash, obj?.GetHashCode() ?? 0));
    }

    public ValueObject GetCopy()
    {
        return (ValueObject)MemberwiseClone();
    }
}


