using System;
using System.Reflection;
using System.Security.Cryptography;


namespace SharedKernel;
public abstract class Enumeration : IComparable
{
    public string Name { get; private set; }

    public int Id { get; private set; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        bool typeMatches = GetType().Equals(obj.GetType());
        bool valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        int absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        T matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        T matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        T? matchingItem = GetAll<T>().FirstOrDefault(predicate) ?? throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    public int CompareTo(object? obj) => Id.CompareTo(((Enumeration?)obj)?.Id ?? 0);

    public static T GetRandomEnumValue<T>() where T : Enumeration
    {
        var values = GetAllEnumValues<T>().ToList();

        if (values.Count == 0)
        {
            throw new InvalidOperationException($"No values found for enumeration type {typeof(T).Name}");
        }

        int index = RandomNumberGenerator.GetInt32(values.Count);
        return values[index];
    }

    public static IEnumerable<T> GetRandomEnumValues<T>(int count = 1) where T : Enumeration
    {
        var values = GetAllEnumValues<T>().ToList();

        if (values.Count == 0)
        {
            throw new InvalidOperationException($"No values found for enumeration type {typeof(T).Name}");

        }

        if (count < 1 || count > values.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Count must be between 1 and {values.Count}");

        }

        // Secure Fisher–Yates shuffle
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(0, i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values.Take(count);
    }

    private static IEnumerable<T> GetAllEnumValues<T>() where T : Enumeration =>
        typeof(T)
            .GetFields(BindingFlags.Public |BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f.FieldType == typeof(T))
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public static bool operator ==(Enumeration? left, Enumeration? right)
         => Equals(left, right);

    public static bool operator !=(Enumeration? left, Enumeration? right)
        => !Equals(left, right);

    public static bool operator <(Enumeration? left, Enumeration? right)
        => left is null ? right is not null : right is not null && left.CompareTo(right) < 0;

    public static bool operator <=(Enumeration? left, Enumeration? right)
        => left is null || right is not null && left.CompareTo(right) <= 0;

    public static bool operator >(Enumeration? left, Enumeration? right)
        => left is not null && (right is null || left.CompareTo(right) > 0);

    public static bool operator >=(Enumeration? left, Enumeration? right)
        => left is null ? right is null : right is null || left.CompareTo(right) >= 0;
}
