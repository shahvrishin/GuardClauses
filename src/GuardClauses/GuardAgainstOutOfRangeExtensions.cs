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
        if (!TryGetUnderlyingTypeValue<T>(input, out object underlyingValue) ||
            !Enum.IsDefined(typeof(T), underlyingValue))
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

    private static bool TryGetUnderlyingTypeValue<T>(int input, out object value) where T : struct, Enum
    {
        switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))))
        {
            case TypeCode.SByte when input >= sbyte.MinValue && input <= sbyte.MaxValue:
                value = (sbyte)input;
                return true;
            case TypeCode.Byte when input >= byte.MinValue && input <= byte.MaxValue:
                value = (byte)input;
                return true;
            case TypeCode.Int16 when input >= short.MinValue && input <= short.MaxValue:
                value = (short)input;
                return true;
            case TypeCode.UInt16 when input >= ushort.MinValue && input <= ushort.MaxValue:
                value = (ushort)input;
                return true;
            case TypeCode.Int32:
                value = input;
                return true;
            case TypeCode.UInt32 when input >= 0:
                value = (uint)input;
                return true;
            case TypeCode.Int64:
                value = (long)input;
                return true;
            case TypeCode.UInt64 when input >= 0:
                value = (ulong)input;
                return true;
            default:
                value = input;
                return false;
        }
    }

    private static InvalidEnumArgumentException CreateInvalidEnumArgumentException<T>(string? parameterName, T input) where T : struct, Enum
    {
        try
        {
            return new InvalidEnumArgumentException(parameterName, Convert.ToInt32(input), typeof(T));
        }
        catch (OverflowException)
        {
            return new InvalidEnumValueException(parameterName, input, typeof(T));
        }
    }

    /// <summary>
    /// An <see cref="InvalidEnumArgumentException" /> for values the framework's int-based constructor cannot
    /// report (a declared value outside the Int32 range of a long or ulong backed enum).
    /// </summary>
    private sealed class InvalidEnumValueException : InvalidEnumArgumentException
    {
        private readonly string? parameterName;

        internal InvalidEnumValueException(string? parameterName, object invalidValue, Type enumType)
            : base(CreateMessage(parameterName, invalidValue, enumType))
        {
            this.parameterName = parameterName;
        }

        public override string? ParamName => parameterName;

        private static string CreateMessage(string? parameterName, object invalidValue, Type enumType)
        {
            var message = $"The value of argument '{parameterName}' ({invalidValue}) is invalid for Enum type '{enumType.Name}'.";
            return parameterName is null ? message : $"{message} (Parameter '{parameterName}')";
        }
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
}
