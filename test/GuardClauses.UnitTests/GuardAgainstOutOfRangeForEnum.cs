using System;
using System.ComponentModel;
using Ardalis.GuardClauses;
using Xunit;

namespace GuardClauses.UnitTests;

public class GuardAgainstOutOfRangeForEnum
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void DoesNothingGivenInRangeValue(int enumValue)
    {
        Guard.Against.EnumOutOfRange<TestEnum>(enumValue, nameof(enumValue));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void ThrowsGivenOutOfRangeValue(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(() => Guard.Against.EnumOutOfRange<TestEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void ThrowsCustomExceptionWhenSuppliedGivenOutOfRangeValue(int enumValue)
    {
        Exception customException = new Exception();
        Assert.Throws<Exception>(() => Guard.Against.EnumOutOfRange<TestEnum>(enumValue, nameof(enumValue),
            exceptionCreator: () => customException));
    }


    [Theory]
    [InlineData(TestEnum.Budgie)]
    [InlineData(TestEnum.Cat)]
    [InlineData(TestEnum.Dog)]
    [InlineData(TestEnum.Fish)]
    [InlineData(TestEnum.Frog)]
    [InlineData(TestEnum.Penguin)]
    public void DoesNothingGivenInRangeEnum(TestEnum enumValue)
    {
        Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue));
    }


    [Theory]
    [InlineData((TestEnum)(-1))]
    [InlineData((TestEnum)6)]
    [InlineData((TestEnum)10)]
    public void ThrowsGivenOutOfRangeEnum(TestEnum enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(() => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData((TestEnum)(-1))]
    [InlineData((TestEnum)6)]
    [InlineData((TestEnum)10)]
    public void ThrowsCustomExceptionWhenSuppliedGivenOutOfRangeEnum(TestEnum enumValue)
    {
        Exception customException = new Exception();
        Assert.Throws<Exception>(() => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue),
            exceptionCreator: () => customException));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void ReturnsExpectedValueGivenInRangeValue(int enumValue)
    {
        var expected = enumValue;
        Assert.Equal(expected, Guard.Against.EnumOutOfRange<TestEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(TestEnum.Budgie)]
    [InlineData(TestEnum.Cat)]
    [InlineData(TestEnum.Dog)]
    [InlineData(TestEnum.Fish)]
    [InlineData(TestEnum.Frog)]
    [InlineData(TestEnum.Penguin)]
    public void ReturnsExpectedValueGivenInRangeEnum(TestEnum enumValue)
    {
        var expected = enumValue;
        Assert.Equal(expected, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(null, "The value of argument 'parameterName' (99) is invalid for Enum type 'TestEnum'. (Parameter 'parameterName')")]
    [InlineData("Invalid enum value", "Invalid enum value")]
    public void ErrorMessageMatchesExpectedWhenInputIsEnum(string? customMessage, string? expectedMessage)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((TestEnum)99, "parameterName", customMessage));
        Assert.NotNull(exception);
        Assert.NotNull(exception.Message);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(null, "The value of argument 'xyz' (99) is invalid for Enum type 'TestEnum'. (Parameter 'xyz')")]
    [InlineData("Invalid enum value", "Invalid enum value")]
    public void ErrorMessageMatchesExpectedWhenInputIsEnumAndParamNameNotExplicitlyProvided(string? customMessage, string? expectedMessage)
    {
        var xyz = (TestEnum)99;
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(xyz, message: customMessage));

        Assert.NotNull(exception);
        Assert.NotNull(exception.Message);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "Please provide correct value")]
    [InlineData("SomeParameter", null)]
    [InlineData(null, "Value must be correct")]
    public void ExceptionParamNameMatchesExpectedWhenInputIsEnum(string? expectedParamName, string? customMessage)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((TestEnum)99, expectedParamName, customMessage));
        Assert.NotNull(exception);
        Assert.Equal(expectedParamName, exception.ParamName);
    }

    [Theory]
    [InlineData(null, "The value of argument 'parameterName' (99) is invalid for Enum type 'TestEnum'. (Parameter 'parameterName')")]
    [InlineData("Invalid enum value", "Invalid enum value")]
    public void ErrorMessageMatchesExpectedWhenInputIsInt(string? customMessage, string? expectedMessage)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<TestEnum>(99, "parameterName", customMessage));
        Assert.NotNull(exception);
        Assert.NotNull(exception.Message);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "Please provide correct value")]
    [InlineData("SomeParameter", null)]
    [InlineData(null, "Value must be correct")]
    public void ExceptionParamNameMatchesExpectedWhenInputIsInt(string? expectedParamName, string? customMessage)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<TestEnum>(99, expectedParamName, customMessage));
        Assert.NotNull(exception);
        Assert.Equal(expectedParamName, exception.ParamName);
    }

    [Theory]
    [InlineData((int)SByteEnum.Min)]
    [InlineData((int)SByteEnum.Max)]
    public void IntOverloadAcceptsDeclaredSByteValues(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<SByteEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-129)]
    [InlineData(128)]
    public void IntOverloadRejectsValuesOutsideSByteRange(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<SByteEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData((int)ByteEnum.Min)]
    [InlineData((int)ByteEnum.Max)]
    public void IntOverloadAcceptsDeclaredByteValues(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<ByteEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(256)]
    public void IntOverloadRejectsValuesOutsideByteRange(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<ByteEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData((int)Int16Enum.Min)]
    [InlineData((int)Int16Enum.Max)]
    public void IntOverloadAcceptsDeclaredInt16Values(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<Int16Enum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-32769)]
    [InlineData(32768)]
    public void IntOverloadRejectsValuesOutsideInt16Range(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<Int16Enum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData((int)UInt16Enum.Min)]
    [InlineData((int)UInt16Enum.Max)]
    public void IntOverloadAcceptsDeclaredUInt16Values(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<UInt16Enum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(65536)]
    public void IntOverloadRejectsValuesOutsideUInt16Range(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<UInt16Enum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void IntOverloadAcceptsDeclaredInt32BoundaryValues(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<Int32BoundaryEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(int.MinValue + 1)]
    [InlineData(int.MaxValue - 1)]
    public void IntOverloadRejectsUndeclaredInt32BoundaryValues(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<Int32BoundaryEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData((int)UInt32Enum.Min)]
    [InlineData((int)UInt32Enum.Mid)]
    public void IntOverloadAcceptsDeclaredUInt32Values(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<UInt32Enum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void IntOverloadRejectsUndeclaredUInt32Values(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<UInt32Enum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData((int)Int64Enum.Small)]
    [InlineData((int)Int64Enum.Other)]
    public void IntOverloadAcceptsDeclaredInt64Values(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<Int64Enum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void IntOverloadRejectsUndeclaredInt64Values(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<Int64Enum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData((int)UInt64Enum.Min)]
    [InlineData((int)UInt64Enum.Small)]
    public void IntOverloadAcceptsDeclaredUInt64Values(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<UInt64Enum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void IntOverloadRejectsUndeclaredUInt64Values(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<UInt64Enum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(SByteEnum.Min)]
    [InlineData(SByteEnum.Max)]
    public void EnumOverloadAcceptsDeclaredSByteValues(SByteEnum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData((SByteEnum)1)]
    [InlineData((SByteEnum)(-1))]
    public void EnumOverloadRejectsUndeclaredSByteValues(SByteEnum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(ByteEnum.Min)]
    [InlineData(ByteEnum.Max)]
    public void EnumOverloadAcceptsDeclaredByteValues(ByteEnum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData((ByteEnum)1)]
    [InlineData((ByteEnum)254)]
    public void EnumOverloadRejectsUndeclaredByteValues(ByteEnum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(Int16Enum.Min)]
    [InlineData(Int16Enum.Max)]
    public void EnumOverloadAcceptsDeclaredInt16Values(Int16Enum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData((Int16Enum)(-32767))]
    [InlineData((Int16Enum)32766)]
    public void EnumOverloadRejectsUndeclaredInt16Values(Int16Enum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(UInt16Enum.Min)]
    [InlineData(UInt16Enum.Max)]
    public void EnumOverloadAcceptsDeclaredUInt16Values(UInt16Enum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData((UInt16Enum)1)]
    [InlineData((UInt16Enum)65534)]
    public void EnumOverloadRejectsUndeclaredUInt16Values(UInt16Enum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(UInt32Enum.Min)]
    [InlineData(UInt32Enum.Mid)]
    [InlineData(UInt32Enum.Max)]
    public void EnumOverloadAcceptsDeclaredUInt32Values(UInt32Enum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData((UInt32Enum)1)]
    [InlineData((UInt32Enum)0xEF000000)]
    public void EnumOverloadRejectsUndeclaredUInt32Values(UInt32Enum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(Int64Enum.Min)]
    [InlineData(Int64Enum.Max)]
    [InlineData(Int64Enum.Small)]
    public void EnumOverloadAcceptsDeclaredInt64Values(Int64Enum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData((Int64Enum)long.MinValue + 1)]
    [InlineData((Int64Enum)long.MaxValue - 1)]
    [InlineData((Int64Enum)((long)int.MaxValue + 1))]
    [InlineData((Int64Enum)((long)int.MinValue - 1))]
    public void EnumOverloadRejectsUndeclaredInt64ValuesOutsideInt32Range(Int64Enum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(UInt64Enum.Min)]
    [InlineData(UInt64Enum.Max)]
    [InlineData(UInt64Enum.Small)]
    public void EnumOverloadAcceptsDeclaredUInt64Values(UInt64Enum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData((UInt64Enum)ulong.MaxValue - 1)]
    [InlineData((UInt64Enum)((ulong)int.MaxValue + 1))]
    [InlineData((UInt64Enum)(0xF000000000000000UL))]
    public void EnumOverloadRejectsUndeclaredUInt64ValuesOutsideInt32Range(UInt64Enum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Fact]
    public void EnumOverloadErrorMessageMatchesExpectedForUInt64ValueOutsideInt32Range()
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((UInt64Enum)ulong.MaxValue - 1, "parameterName"));
        Assert.Equal("parameterName", exception.ParamName);
        Assert.Equal(
            "The value of argument 'parameterName' (18446744073709551614) is invalid for Enum type 'UInt64Enum'. (Parameter 'parameterName')",
            exception.Message);
    }

    [Fact]
    public void EnumOverloadErrorMessageMatchesExpectedForInt64ValueOutsideInt32Range()
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((Int64Enum)long.MinValue + 1, "parameterName"));
        Assert.Equal("parameterName", exception.ParamName);
        Assert.Equal(
            "The value of argument 'parameterName' (-9223372036854775807) is invalid for Enum type 'Int64Enum'. (Parameter 'parameterName')",
            exception.Message);
    }

    [Fact]
    public void EnumOverloadThrowsCustomExceptionForInt64ValueOutsideInt32Range()
    {
        Exception customException = new Exception();
        Assert.Throws<Exception>(() => Guard.Against.EnumOutOfRange(
            (Int64Enum)long.MaxValue - 1, nameof(Int64Enum), exceptionCreator: () => customException));
    }

    [Fact]
    public void EnumOverloadThrowsCustomMessageForUInt64ValueOutsideInt32Range()
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((UInt64Enum)ulong.MaxValue - 1, "parameterName", "Invalid enum value"));
        Assert.Equal("Invalid enum value", exception.Message);
    }

    [Theory]
    [InlineData(FlagEnum.None)]
    [InlineData(FlagEnum.A)]
    [InlineData(FlagEnum.B)]
    public void EnumOverloadAcceptsDeclaredFlagValues(FlagEnum enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(FlagEnum.A | FlagEnum.B)]
    [InlineData((FlagEnum)4)]
    public void EnumOverloadRejectsCombinedOrUndeclaredFlagValues(FlagEnum enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public void IntOverloadRejectsCombinedOrUndeclaredFlagValues(int enumValue)
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<FlagEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }
}


public enum TestEnum
{
    Cat = 0,
    Dog = 1,
    Fish = 2,
    Budgie = 3,
    Penguin = 4,
    Frog = 5
}

public enum SByteEnum : sbyte
{
    Min = sbyte.MinValue,
    Max = sbyte.MaxValue
}

public enum ByteEnum : byte
{
    Min = byte.MinValue,
    Max = byte.MaxValue
}

public enum Int16Enum : short
{
    Min = short.MinValue,
    Max = short.MaxValue
}

public enum UInt16Enum : ushort
{
    Min = ushort.MinValue,
    Max = ushort.MaxValue
}

public enum Int32BoundaryEnum
{
    Min = int.MinValue,
    Max = int.MaxValue
}

public enum UInt32Enum : uint
{
    Min = uint.MinValue,
    Mid = int.MaxValue,
    Max = 4000000000
}

public enum Int64Enum : long
{
    Min = long.MinValue,
    Max = long.MaxValue,
    Small = 0,
    Other = 5
}

public enum UInt64Enum : ulong
{
    Min = ulong.MinValue,
    Max = ulong.MaxValue,
    Small = 5
}

[Flags]
public enum FlagEnum
{
    None = 0,
    A = 1,
    B = 2
}