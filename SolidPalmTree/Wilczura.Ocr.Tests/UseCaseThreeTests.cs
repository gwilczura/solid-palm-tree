using FluentAssertions;

namespace Wilczura.Ocr.Tests;

public class UseCaseThreeTests
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
        var result = Parser.GetParsingResult(entry);
        var textResult = string.Join(string.Empty, result.Select(x => x.ToString()));

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When49006771Strange_ThenItIsIllegible()
    {
        // Arrange
        var entry = @"
    _  _  _  _  _  _     _ 
|_||_|| || ||_   |  |  | _ 
  | _||_||_||_|  |  |  | _|
";
        var expextedResult = "49006771? ILL";
        // Act
        var result = Parser.GetParsingResult(entry);
        var textResult = string.Join(string.Empty, result.Select(x => x.ToString()));

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When1234X678X_ThenItIsIllegible()
    {
        // Arrange
        var entry = @"
    _  _     _  _  _  _  _ 
  | _| _||_| _ |_   ||_||_|
  ||_  _|  | _||_|  ||_| _ 
";
        var expextedResult = "1234?678? ILL";
        // Act
        var result = Parser.GetParsingResult(entry);
        var textResult = string.Join(string.Empty, result.Select(x => x.ToString()));

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When111111111_ThenItIsIllegible()
    {
        // Arrange
        var entry = @"
                           
  |  |  |  |  |  |  |  |  |
  |  |  |  |  |  |  |  |  |
";
        var expextedResult = "111111111 ERR";
        // Act
        var result = Parser.GetParsingResult(entry);
        var textResult = string.Join(string.Empty, result.Select(x => x.ToString()));

        // Assert
        textResult.Should().Be(expextedResult);
    }
}
