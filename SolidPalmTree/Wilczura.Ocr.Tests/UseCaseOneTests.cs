using FluentAssertions;

namespace Wilczura.Ocr.Tests;

public class UseCaseOneTests
{
    [Theory]
    [InlineData("000000000")]
    [InlineData("123456789")]
    [InlineData("711111111")]
    [InlineData("777777177")]
    [InlineData("200800000")]
    [InlineData("000000051")]
    [InlineData("490867715")]
    public void WhenNumberCanBeParsed_ThenItIsParsed(string accountNumber)
    {
        // Arrange
        var digits = accountNumber.Select(x => Consts.DigitsMap[int.Parse(x.ToString())]).ToList();
        var entry = Parser.GetEntryFromDigits(digits);
        // Act
        var result = Parser.GetNumber(entry);
        var textResult = string.Join(string.Empty, result.Select(x => x.ToString()));

        // Assert
        textResult.Should().Be(accountNumber);
    }
}
