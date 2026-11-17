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
    Zero = 0,
    Max = sbyte.MaxValue
}

public enum ByteEnum : byte
{
    Zero = 0,
    Ten = 10,
    Max = byte.MaxValue
}

public enum ShortEnum : short
{
    Min = short.MinValue,
    Zero = 0,
    Max = short.MaxValue
}

public enum UShortEnum : ushort
{
    Zero = 0,
    Ten = 10,
    Max = ushort.MaxValue
}

public enum IntEnum
{
    Min = int.MinValue,
    Zero = 0,
    Max = int.MaxValue
}

public enum UIntEnum : uint
{
    Zero = 0,
    Ten = 10,
    Max = uint.MaxValue
}

public enum LongEnum : long
{
    Min = long.MinValue,
    Zero = 0,
    Max = long.MaxValue
}

public enum ULongEnum : ulong
{
    Zero = 0,
    Max = ulong.MaxValue
}

[Flags]
public enum FlagsEnum
{
    None = 0,
    A = 1,
    B = 2,
    AB = 3,
    C = 4
}

public class GuardAgainstOutOfRangeForEnumWithNonIntBackingTypes
{
    [Theory]
    [InlineData(-128)]
    [InlineData(0)]
    [InlineData(127)]
    public void ReturnsExpectedValueGivenDeclaredValueForSByteBackedEnum(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<SByteEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-129)]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(128)]
    public void ThrowsGivenUndeclaredOrUnrepresentableValueForSByteBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<SByteEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(255)]
    public void ReturnsExpectedValueGivenDeclaredValueForByteBackedEnum(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<ByteEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(256)]
    [InlineData(300)]
    public void ThrowsGivenUndeclaredOrUnrepresentableValueForByteBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<ByteEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(-32768)]
    [InlineData(0)]
    [InlineData(32767)]
    public void ReturnsExpectedValueGivenDeclaredValueForShortBackedEnum(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<ShortEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-32769)]
    [InlineData(1)]
    [InlineData(32768)]
    public void ThrowsGivenUndeclaredOrUnrepresentableValueForShortBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<ShortEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(65535)]
    public void ReturnsExpectedValueGivenDeclaredValueForUShortBackedEnum(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<UShortEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(65536)]
    public void ThrowsGivenUndeclaredOrUnrepresentableValueForUShortBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<UShortEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    public void ReturnsExpectedValueGivenDeclaredValueForIntBackedEnum(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<IntEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void ThrowsGivenUndeclaredValueForIntBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<IntEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void ReturnsExpectedValueGivenDeclaredValueForUIntBackedEnum(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<UIntEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ThrowsGivenUndeclaredOrUnrepresentableValueForUIntBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<UIntEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Fact]
    public void ReturnsExpectedValueGivenDeclaredValueForLongAndULongBackedEnums()
    {
        Assert.Equal(0, Guard.Against.EnumOutOfRange<LongEnum>(0, "value"));
        Assert.Equal(0, Guard.Against.EnumOutOfRange<ULongEnum>(0, "value"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ThrowsGivenUndeclaredValueForLongBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<LongEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ThrowsGivenUndeclaredOrUnrepresentableValueForULongBackedEnum(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<ULongEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnsExpectedValueGivenDeclaredFlagValues(int enumValue)
    {
        Assert.Equal(enumValue, Guard.Against.EnumOutOfRange<FlagsEnum>(enumValue, nameof(enumValue)));
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public void ThrowsGivenFlagsCombinationThatIsNotDeclared(int enumValue)
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<FlagsEnum>(enumValue, nameof(enumValue)));
        Assert.Equal(nameof(enumValue), exception.ParamName);
    }

    [Fact]
    public void ThrowsGivenFlagsCombinationThatIsNotDeclaredForEnumOverload()
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange(FlagsEnum.A | FlagsEnum.C, "value"));
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void CustomMessageIsUsedGivenValueNotRepresentableByBackingType()
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange<ByteEnum>(300, "value", "Invalid enum value"));
        Assert.Equal("Invalid enum value", exception.Message);
    }

    [Fact]
    public void CustomMessageIsUsedGivenEnumValueOutsideInt32Range()
    {
        var exception = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((LongEnum)((long)int.MaxValue + 1), "value", "Invalid enum value"));
        Assert.Equal("Invalid enum value", exception.Message);
    }

    [Fact]
    public void CustomExceptionIsThrownGivenValueNotRepresentableByBackingType()
    {
        Exception customException = new Exception();
        var thrown = Assert.Throws<Exception>(
            () => Guard.Against.EnumOutOfRange<ByteEnum>(300, "value", exceptionCreator: () => customException));
        Assert.Same(customException, thrown);
    }

    [Fact]
    public void CustomExceptionIsThrownGivenEnumValueOutsideInt32Range()
    {
        Exception customException = new Exception();
        var thrown = Assert.Throws<Exception>(
            () => Guard.Against.EnumOutOfRange((ULongEnum)((ulong)int.MaxValue + 1), "value",
                exceptionCreator: () => customException));
        Assert.Same(customException, thrown);
    }

    [Fact]
    public void ReturnsExpectedEnumValuesForNonIntBackedEnums()
    {
        Assert.Equal(SByteEnum.Min, Guard.Against.EnumOutOfRange(SByteEnum.Min, "value"));
        Assert.Equal(SByteEnum.Max, Guard.Against.EnumOutOfRange(SByteEnum.Max, "value"));
        Assert.Equal(ByteEnum.Max, Guard.Against.EnumOutOfRange(ByteEnum.Max, "value"));
        Assert.Equal(ShortEnum.Min, Guard.Against.EnumOutOfRange(ShortEnum.Min, "value"));
        Assert.Equal(ShortEnum.Max, Guard.Against.EnumOutOfRange(ShortEnum.Max, "value"));
        Assert.Equal(UShortEnum.Max, Guard.Against.EnumOutOfRange(UShortEnum.Max, "value"));
        Assert.Equal(IntEnum.Min, Guard.Against.EnumOutOfRange(IntEnum.Min, "value"));
        Assert.Equal(IntEnum.Max, Guard.Against.EnumOutOfRange(IntEnum.Max, "value"));
        Assert.Equal(UIntEnum.Max, Guard.Against.EnumOutOfRange(UIntEnum.Max, "value"));
        Assert.Equal(LongEnum.Min, Guard.Against.EnumOutOfRange(LongEnum.Min, "value"));
        Assert.Equal(LongEnum.Max, Guard.Against.EnumOutOfRange(LongEnum.Max, "value"));
        Assert.Equal(ULongEnum.Max, Guard.Against.EnumOutOfRange(ULongEnum.Max, "value"));
        Assert.Equal(FlagsEnum.AB, Guard.Against.EnumOutOfRange(FlagsEnum.AB, "value"));
    }

    [Fact]
    public void ThrowsGivenUndeclaredEnumValuesWithinInt32Range()
    {
        var sbyteException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((SByteEnum)1, "value"));
        Assert.Equal("value", sbyteException.ParamName);
        Assert.Contains("SByteEnum", sbyteException.Message);
        Assert.Contains("(Parameter 'value')", sbyteException.Message);

        var byteException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((ByteEnum)1, "value"));
        Assert.Equal("value", byteException.ParamName);

        var shortException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((ShortEnum)1, "value"));
        Assert.Equal("value", shortException.ParamName);

        var ushortException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((UShortEnum)1, "value"));
        Assert.Equal("value", ushortException.ParamName);

        var intException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((IntEnum)1, "value"));
        Assert.Equal("value", intException.ParamName);

        var uintException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((UIntEnum)5, "value"));
        Assert.Equal("value", uintException.ParamName);

        var longException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((LongEnum)(-1L), "value"));
        Assert.Equal("value", longException.ParamName);

        var ulongException = Assert.Throws<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((ULongEnum)5UL, "value"));
        Assert.Equal("value", ulongException.ParamName);

        var uintAboveInt32Exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((UIntEnum)((uint)int.MaxValue + 1), "value"));
        Assert.Equal("value", uintAboveInt32Exception.ParamName);
    }

    [Fact]
    public void ThrowsInvalidEnumArgumentExceptionGivenLongValueOutsideInt32Range()
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((LongEnum)((long)int.MaxValue + 1), "parameterName"));

        Assert.Contains("'parameterName'", exception.Message);
        Assert.Contains("LongEnum", exception.Message);
        Assert.Contains("(Parameter 'parameterName')", exception.Message);
        Assert.Equal("parameterName", exception.ParamName);
    }

    [Fact]
    public void ThrowsInvalidEnumArgumentExceptionGivenULongValueOutsideInt32Range()
    {
        var exception = Assert.ThrowsAny<InvalidEnumArgumentException>(
            () => Guard.Against.EnumOutOfRange((ULongEnum)((ulong)int.MaxValue + 1), "parameterName"));

        Assert.Contains("'parameterName'", exception.Message);
        Assert.Contains("ULongEnum", exception.Message);
        Assert.Contains("(Parameter 'parameterName')", exception.Message);
        Assert.Equal("parameterName", exception.ParamName);
    }
}
