using System.Reflection;

namespace RaspCameraLibrary.Helpers;

/// <summary>
/// Custom enumeration.
/// </summary>
public abstract class Enumeration<TValue> : Stringable, IComparable where TValue : IComparable, IComparable<TValue>
{
    /// <summary>
    /// The value.
    /// </summary>
    public TValue Value { get; private set; }

    /// <summary>
    /// The description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Create a new value for enumeration.
    /// </summary>
    protected Enumeration(TValue value, string? description = null) => (Value, Description) = (value, description);

    /// <summary>
    /// Convert to string.
    /// </summary>
    public override string? ToString() => Value?.ToString();

    /// <summary>
    /// Get all values.
    /// </summary>
    public static IEnumerable<TEnumeration> GetAll<TEnumeration>() where TEnumeration : Enumeration<TValue>
    {
        PropertyInfo[] Values = typeof(TEnumeration)
            .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (PropertyInfo Value in Values)
        {
            if (Value.GetValue(null) is TEnumeration Enumeration)
            {
                yield return Enumeration;
            }
        }
    }

    /// <summary>
    /// Get value from string.
    /// </summary>
    public static TEnumeration? Get<TEnumeration>(string value) where TEnumeration : Enumeration<TValue>
    {
        PropertyInfo[] Values = typeof(TEnumeration)
            .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (PropertyInfo Value in Values)
        {
            if (Value.GetValue(null) is TEnumeration Enumeration && Enumeration.Value.Equals(value))
            {
                return Enumeration;
            }
        }

        return null;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration<TValue> otherValue)
        {
            return false;
        }

        bool typeMatches = GetType().Equals(obj.GetType());
        bool valueMatches = Value.Equals(otherValue.Value);

        return typeMatches && valueMatches;
    }

    /// <summary>
    /// CompareTo
    /// </summary>
    public int CompareTo(object? other)
    {
        return other is null ? throw new ArgumentNullException(nameof(other)) : Value.CompareTo(((Enumeration<TValue>)other).Value);
    }

    /// <summary>
    /// GetHashCode
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}