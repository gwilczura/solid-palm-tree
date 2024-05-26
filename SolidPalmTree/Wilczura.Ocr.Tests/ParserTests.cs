using AutoFixture.Xunit2;
using FluentAssertions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Wilczura.Ocr.Tests;

public static class ParserTests
{
    public class TheParseEntryToDigitsMethod
    {
        [Fact]
        public void WhenInputIs123456789_ThenOutputIs123456789()
        {
            // Arrange
            var entry = @"
    _  _     _  _  _  _  _ 
  | _| _||_||_ |_   ||_||_|
  ||_  _|  | _||_|  ||_| _|
";

            // Act
            var result = Parser.ParseEntryToDigits(entry);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result[0].Should().Be(Consts.One);
            result[1].Should().Be(Consts.Two);
            result[2].Should().Be(Consts.Three);
            result[3].Should().Be(Consts.Four);
            result[4].Should().Be(Consts.Five);
            result[5].Should().Be(Consts.Six);
            result[6].Should().Be(Consts.Seven);
            result[7].Should().Be(Consts.Eight);
            result[8].Should().Be(Consts.Nine);
        }

        [Fact]
        public void WhenInputIs234567890_ThenOutputIs234567890()
        {
            // Arrange
            var entry = @"
 _  _     _  _  _  _  _  _ 
 _| _||_||_ |_   ||_||_|| |
|_  _|  | _||_|  ||_| _||_|
";

            // Act
            var result = Parser.ParseEntryToDigits(entry);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result[0].Should().Be(Consts.Two);
            result[1].Should().Be(Consts.Three);
            result[2].Should().Be(Consts.Four);
            result[3].Should().Be(Consts.Five);
            result[4].Should().Be(Consts.Six);
            result[5].Should().Be(Consts.Seven);
            result[6].Should().Be(Consts.Eight);
            result[7].Should().Be(Consts.Nine);
            result[8].Should().Be(Consts.Zero);
        }
    }

    public class TheIsNearMethod
    {
        [Fact]
        public void WhenIsNear_ThenOutputIsTrue()
        {
            // Arrange
            var digitOne = "123456789";
            var digitTwo = "123456780";

            // Act
            var result = Parser.IsNearByOneSign(digitOne, digitTwo);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void WhenIsTheSame_ThenOutputIsFalse()
        {
            // Arrange
            var digitOne = "123456789";

            // Act
            var result = Parser.IsNearByOneSign(digitOne, digitOne);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void WhenIsNotNewarByOneSign_ThenOutputIsFalse()
        {
            // Arrange
            var digitOne = "123456789";
            var digitTwo = "023456780";

            // Act
            var result = Parser.IsNearByOneSign(digitOne, digitTwo);

            // Assert
            result.Should().BeFalse();
        }
    }


    public class IsAccountNumberMethod
    {
        [Theory]
        [InlineData("000000000")]
        [InlineData("123456789")]
        [InlineData("711111111")]
        [InlineData("777777177")]
        [InlineData("200800000")]
        [InlineData("000000051")]
        [InlineData("490867715")]
        public void WhenIsAccountNumber_ThenOutputIsTrue(string accountNumber)
        {
            // Arrange
            var number = accountNumber.Select(x => (int?)int.Parse(new string(x, 1))).ToList();

            // Act
            var result = Parser.IsAccountNumber(number);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("111111111")]
        [InlineData("777777777")]
        [InlineData("200000000")]
        [InlineData("333333333")]
        [InlineData("888888888")]
        [InlineData("555555555")]
        [InlineData("666666666")]
        [InlineData("999999999")]
        [InlineData("490067715")]
        public void WhenIsNotAccountNumber_ThenOutputIsFalse(string accountNumber)
        {
            // Arrange
            var number = accountNumber.Select(x => (int?)int.Parse(x.ToString())).ToList();

            // Act
            var result = Parser.IsAccountNumber(number);

            // Assert
            result.Should().BeFalse();
        }
    }

    public class TheGetEntryFromDigitsMethod
    {
        [Fact]
        public void WhenInputIs123456789_ThenOutputIs123456789()
        {
            // Arrange
            var expectedResult = @"
    _  _     _  _  _  _  _ 
  | _| _||_||_ |_   ||_||_|
  ||_  _|  | _||_|  ||_| _|
";

            var digits = "123456789"
                .Select(a => int.Parse(a.ToString()))
                .Select(a => Consts.DigitsMap[a])
                .ToList();

            // Act
            var result = Parser.GetEntryFromDigits(digits);
            // to simplify display
            result = Environment.NewLine + result;

            // Assert
            result.Should().Be(expectedResult);
        }


        [Fact]
        public void WhenInputIs234567890_ThenOutputIs234567890()
        {
            // Arrange
            var expectedResult = @"
 _  _     _  _  _  _  _  _ 
 _| _||_||_ |_   ||_||_|| |
|_  _|  | _||_|  ||_| _||_|
";

            var digits = "234567890"
                .Select(a => int.Parse(a.ToString()))
                .Select(a => Consts.DigitsMap[a])
                .ToList();

            // Act
            var result = Parser.GetEntryFromDigits(digits);
            // to simplify display
            result = Environment.NewLine + result;

            // Assert
            result.Should().Be(expectedResult);
        }
    }

    public class TheGetCombinationsMethod
    {


        [Fact]
        public void WhenInputIsSpecified_ThenOutputIsAsExpected()
        {
            // Arrange
            var input = new List<List<int>>
            {
                new() { 1, 2, 3},
                new() { 4, 5, 6},
                new() { 7, 8, 9},
            };
            var expectedResult = new List<List<int>>
            {
                new() { 1, 4, 7},
                new() { 1, 4, 8},
                new() { 1, 4, 9},
                new() { 1, 5, 7},
                new() { 1, 5, 8},
                new() { 1, 5, 9},
                new() { 1, 6, 7},
                new() { 1, 6, 8},
                new() { 1, 6, 9},
                new() { 2, 4, 7},
                new() { 2, 4, 8},
                new() { 2, 4, 9},
                new() { 2, 5, 7},
                new() { 2, 5, 8},
                new() { 2, 5, 9},
                new() { 2, 6, 7},
                new() { 2, 6, 8},
                new() { 2, 6, 9},
                new() { 3, 4, 7},
                new() { 3, 4, 8},
                new() { 3, 4, 9},
                new() { 3, 5, 7},
                new() { 3, 5, 8},
                new() { 3, 5, 9},
                new() { 3, 6, 7},
                new() { 3, 6, 8},
                new() { 3, 6, 9},
            };;

            // Act
            var result = Parser.GetCombinations(input, []);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}