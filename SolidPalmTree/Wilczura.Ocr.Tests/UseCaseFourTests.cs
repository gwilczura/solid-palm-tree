using FluentAssertions;

namespace Wilczura.Ocr.Tests;

public class UseCaseFourTests
{
    [Fact]
    public void When000000051_ThenItIsParsed()
    {
        // Arrange
        var expextedResult = "000000051";
        var entry = @"
 _  _  _  _  _  _  _  _    
| || || || || || || ||_   |
|_||_||_||_||_||_||_| _|  |
";
        // Act
        var result = Parser.GetParsingResultWithDeviation(entry);
        var textResult = string.Join(string.Empty, result.Select(x => x.ToString()));

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When888888888_ThenItIsParsedAsAmbigous()
    {
        // Arrange
        var expextedResult = "888888888 AMB ['888886888', '888888880', '888888988']";
        var entry = @"
 _  _  _  _  _  _  _  _  _ 
|_||_||_||_||_||_||_||_||_|
|_||_||_||_||_||_||_||_||_|
";
        // Act
        var result = Parser.GetParsingResultWithDeviation(entry);
        var textResult = string.Join(string.Empty, result.Select(x => x.ToString()));

        // Assert
        textResult.Should().Be(expextedResult);
    }
}
