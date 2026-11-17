using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ardalis.GuardClauses;

public static partial class GuardClauseExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentException" /> or a custom <see cref="Exception" /> if string <paramref name="input"/> length is out of range.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="minLength"></param>
    /// <param name="maxLength"></param>
    /// <param name="parameterName"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is not negative.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static string LengthOutOfRange(this IGuardClause guardClause,
        string input,
        int minLength,
        int maxLength,
        [CallerArgumentExpression("input")] string? parameterName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null)
    {
        Guard.Against.Negative<int>(maxLength - minLength, parameterName: "min or max length",
            message: "Min length must be equal or less than max length.", exceptionCreator: exceptionCreator);
        Guard.Against.StringTooShort(input, minLength, nameof(minLength), exceptionCreator: exceptionCreator);
        Guard.Against.StringTooLong(input, maxLength, nameof(maxLength), exceptionCreator: exceptionCreator);

        return input;
    }

    /// <summary>
    /// Throws an <see cref="InvalidEnumArgumentException" /> or a custom <see cref="Exception" /> if <paramref name="input"/> is not a valid enum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is not out of range.</returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static int EnumOutOfRange<T>(this IGuardClause guardClause,
        int input,
        [CallerArgumentExpression("input")] string? parameterName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) where T : struct, Enum
    {
        if (!TryConvertToUnderlyingType(typeof(T), input, out var enumValue) || !Enum.IsDefined(typeof(T), enumValue!))
        {
            var exception = exceptionCreator?.Invoke();
            if (string.IsNullOrEmpty(message))
            {
                throw exception ?? new InvalidEnumArgumentException(parameterName, input, typeof(T));
            }
            throw exception ?? new InvalidEnumArgumentException(message);
        }

        return input;
    }

    /// <summary>
    /// Throws an <see cref="InvalidEnumArgumentException" /> or a custom <see cref="Exception" /> if <paramref name="input"/> is not a valid enum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is not out of range.</returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static T EnumOutOfRange<T>(this IGuardClause guardClause,
        T input,
        [CallerArgumentExpression("input")] string? parameterName = null,
        string? message = null,
        Func<Exception>? exceptionCreator = null) where T : struct, Enum
    {
        if (!Enum.IsDefined(typeof(T), input))
        {
            var exception = exceptionCreator?.Invoke();
            if (string.IsNullOrEmpty(message))
            {
                throw exception ?? CreateInvalidEnumArgumentException(parameterName, input);
            }
            throw exception ?? new InvalidEnumArgumentException(message);
        }

        return input;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException" /> or a custom <see cref="Exception" /> if
    /// any <paramref name="input"/>'s item is less than <paramref name="rangeFrom"/> or greater than <paramref name="rangeTo"/>.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// <param name="rangeFrom"></param>
    /// <param name="rangeTo"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if any item is not out of range.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="Exception"></exception>
    public static IEnumerable<T> OutOfRange<T>(this IGuardClause guardClause,
        IEnumerable<T> input,
        string parameterName,
        T rangeFrom, T rangeTo,
        string? message = null,
        Func<Exception>? exceptionCreator = null) where T : IComparable, IComparable<T>
    {
        if (rangeFrom.CompareTo(rangeTo) > 0)
        {
            throw exceptionCreator?.Invoke() ?? 
                new ArgumentException(message ?? $"{nameof(rangeFrom)} should be less or equal than {nameof(rangeTo)}", parameterName);
        }

        if (input.Any(x => x.CompareTo(rangeFrom) < 0 || x.CompareTo(rangeTo) > 0))
        {
            if (string.IsNullOrEmpty(message))
            {
                throw exceptionCreator?.Invoke() ?? 
                    new ArgumentOutOfRangeException(parameterName, message ?? $"Input {parameterName} had out of range item(s)");
            }
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(parameterName, message);
        }

        return input;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException" /> or a custom <see cref="Exception" /> if <paramref name="input" /> is null.
    /// Throws an <see cref="ArgumentOutOfRangeException" /> or a custom <see cref="Exception" /> if <paramref name="input" /> is not in the range of valid SqlDateTime values.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is in the range of valid SqlDateTime values.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="Exception"></exception>
    public static DateTime NullOrOutOfSQLDateRange(this IGuardClause guardClause,
         [NotNull][ValidatedNotNull] DateTime? input,
         [CallerArgumentExpression("input")] string? parameterName = null,
         string? message = null, Func<Exception>? exceptionCreator = null)
    {
        guardClause.Null(input, nameof(input), exceptionCreator: exceptionCreator);
        return OutOfSQLDateRange(guardClause, input.Value, parameterName, message, exceptionCreator);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException" /> or a custom <see cref="Exception" /> if <paramref name="input" /> is not in the range of valid SqlDateTime values.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is in the range of valid SqlDateTime values.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="Exception"></exception>
    public static DateTime OutOfSQLDateRange(this IGuardClause guardClause,
        DateTime input,
        [CallerArgumentExpression("input")] string? parameterName = null,
        string? message = null, Func<Exception>? exceptionCreator = null)
    {
        // System.Data is unavailable in .NET Standard so we can't use SqlDateTime.
        const long sqlMinDateTicks = 552877920000000000;
        const long sqlMaxDateTicks = 3155378975999970000;

        return NullOrOutOfRangeInternal<DateTime>(guardClause, input, parameterName, new DateTime(sqlMinDateTicks), new DateTime(sqlMaxDateTicks), message, exceptionCreator);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException" /> or a custom <see cref="Exception" /> if <paramref name="input"/> is less than <paramref name="rangeFrom"/> or greater than <paramref name="rangeTo"/>.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// <param name="rangeFrom"></param>
    /// <param name="rangeTo"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is not out of range.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="Exception"></exception>
    public static T OutOfRange<T>(this IGuardClause guardClause,
        T input,
        string parameterName,
        [NotNull][ValidatedNotNull] T rangeFrom,
        [NotNull][ValidatedNotNull] T rangeTo,
        string? message = null, Func<Exception>? exceptionCreator = null) where T : IComparable, IComparable<T>
    {
        return NullOrOutOfRangeInternal<T>(guardClause, input, parameterName, rangeFrom, rangeTo, message, exceptionCreator);
    }


    /// <summary>
    /// Throws an <see cref="ArgumentNullException" /> or a custom <see cref="Exception" /> if <paramref name="input" /> is null.
    /// Throws an <see cref="ArgumentOutOfRangeException" /> or a custom <see cref="Exception" /> if <paramref name="input"/> is less than <paramref name="rangeFrom"/> or greater than <paramref name="rangeTo"/>.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// <param name="rangeFrom"></param>
    /// <param name="rangeTo"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is not not null or out of range.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="Exception"></exception>
    public static T NullOrOutOfRange<T>(this IGuardClause guardClause,
        [NotNull][ValidatedNotNull] T? input,
        string parameterName,
        [NotNull][ValidatedNotNull] T rangeFrom,
        [NotNull][ValidatedNotNull] T rangeTo,
        string? message = null, Func<Exception>? exceptionCreator = null) where T : IComparable<T>
    {
        guardClause.Null(input, nameof(input),exceptionCreator: exceptionCreator);
        return NullOrOutOfRangeInternal(guardClause, input, parameterName, rangeFrom, rangeTo, message, exceptionCreator);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException" /> or a custom <see cref="Exception" /> if <paramref name="input" /> is null.
    /// Throws an <see cref="ArgumentOutOfRangeException" /> or a custom <see cref="Exception" /> if <paramref name="input"/> is less than <paramref name="rangeFrom"/> or greater than <paramref name="rangeTo"/>.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="input"></param>
    /// <param name="parameterName"></param>
    /// <param name="rangeFrom"></param>
    /// <param name="rangeTo"></param>
    /// <param name="message">Optional. Custom error message</param>
    /// <param name="exceptionCreator"></param>
    /// <returns><paramref name="input" /> if the value is not not null or out of range.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="Exception"></exception>
    public static T NullOrOutOfRange<T>(this IGuardClause guardClause,
        [NotNull][ValidatedNotNull] T? input,
        string parameterName,
        [NotNull][ValidatedNotNull] T rangeFrom,
        [NotNull][ValidatedNotNull] T rangeTo,
        string? message = null, Func<Exception>? exceptionCreator = null) where T : struct, IComparable<T>
    {
        guardClause.Null(input, nameof(input), exceptionCreator: exceptionCreator);
        return NullOrOutOfRangeInternal<T>(guardClause, input.Value, parameterName, rangeFrom, rangeTo, message, exceptionCreator);
    }

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static T NullOrOutOfRangeInternal<T>(this IGuardClause guardClause,
        [NotNull][ValidatedNotNull] T? input,
        string? parameterName,
        [NotNull][ValidatedNotNull] T? rangeFrom,
        [NotNull][ValidatedNotNull] T? rangeTo,
        string? message = null,
        Func<Exception>? exceptionCreator = null) where T : IComparable<T>?
    {
        Guard.Against.Null(input, nameof(input));
        Guard.Against.Null(parameterName, nameof(parameterName));
        Guard.Against.Null(rangeFrom, nameof(rangeFrom));
        Guard.Against.Null(rangeTo, nameof(rangeTo));

        if (rangeFrom.CompareTo(rangeTo) > 0)
        {
            throw new ArgumentException(message ?? $"{nameof(rangeFrom)} should be less or equal than {nameof(rangeTo)}", parameterName);
        }

        if (input.CompareTo(rangeFrom) < 0 || input.CompareTo(rangeTo) > 0)
        {
            Exception? exception = exceptionCreator?.Invoke();

            if (string.IsNullOrEmpty(message))
            {
                throw exception ?? new ArgumentOutOfRangeException(parameterName, $"Input {parameterName} was out of range");
            }
            throw exception ?? new ArgumentOutOfRangeException(parameterName, message);
        }

        return input;
    }

    // Enum.IsDefined(Type, object) only accepts a boxed value of the enum's exact underlying type, so an int input
    // has to be converted for enums backed by anything but int. Values the backing type cannot represent are
    // reported as out of range rather than being narrowed or wrapped into a declared value.
    private static bool TryConvertToUnderlyingType(Type enumType, int value, out object? convertedValue)
    {
        try
        {
            convertedValue = Convert.ChangeType(value, Enum.GetUnderlyingType(enumType));
            return true;
        }
        catch (OverflowException)
        {
            convertedValue = null;
            return false;
        }
    }

    // The (parameterName, int, Type) constructor of InvalidEnumArgumentException needs an int, which enum values
    // outside the Int32 range cannot provide, so those fall back to its message constructor with the same shape.
    private static InvalidEnumArgumentException CreateInvalidEnumArgumentException<T>(string? parameterName, T input)
        where T : struct, Enum
    {
        switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
        {
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
                return new InvalidEnumArgumentException(parameterName, Convert.ToInt32(input), typeof(T));
            case TypeCode.UInt32:
                var uintValue = Convert.ToUInt32(input);
                if (uintValue <= int.MaxValue)
                {
                    return new InvalidEnumArgumentException(parameterName, (int)uintValue, typeof(T));
                }
                break;
            case TypeCode.Int64:
                var longValue = Convert.ToInt64(input);
                if (longValue >= int.MinValue && longValue <= int.MaxValue)
                {
                    return new InvalidEnumArgumentException(parameterName, (int)longValue, typeof(T));
                }
                break;
            case TypeCode.UInt64:
                var ulongValue = Convert.ToUInt64(input);
                if (ulongValue <= int.MaxValue)
                {
                    return new InvalidEnumArgumentException(parameterName, (int)ulongValue, typeof(T));
                }
                break;
        }

        var message = $"The value of argument '{parameterName}' ({input}) is invalid for Enum type '{typeof(T).Name}'.";
        return parameterName is null
            ? new InvalidEnumArgumentException(message)
            : new InvalidEnumArgumentException($"{message} (Parameter '{parameterName}')");
    }
}
