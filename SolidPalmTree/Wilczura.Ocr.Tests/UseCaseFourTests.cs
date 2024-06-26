﻿using FluentAssertions;

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
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When111111111_ThenItIsParsedAs711111111()
    {
        // Arrange
        var expextedResult = "711111111";
        var entry = @"
                           
  |  |  |  |  |  |  |  |  |
  |  |  |  |  |  |  |  |  |
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When777777777_ThenItIsParsedAs777777177()
    {
        // Arrange
        var expextedResult = "777777177";
        var entry = @"
 _  _  _  _  _  _  _  _  _ 
  |  |  |  |  |  |  |  |  |
  |  |  |  |  |  |  |  |  |
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When200000000_ThenItIsParsedAs200800000()
    {
        // Arrange
        var expextedResult = "200800000";
        var entry = @"
 _  _  _  _  _  _  _  _  _ 
 _|| || || || || || || || |
|_ |_||_||_||_||_||_||_||_|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When333333333_ThenItIsParsedAs333393333()
    {
        // Arrange
        var expextedResult = "333393333";
        var entry = @"
 _  _  _  _  _  _  _  _  _ 
 _| _| _| _| _| _| _| _| _|
 _| _| _| _| _| _| _| _| _|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

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
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When555555555_ThenItIsParsedAsAmbigous()
    {
        // Arrange
        var expextedResult = "555555555 AMB ['555655555', '559555555']";
        var entry = @"
 _  _  _  _  _  _  _  _  _ 
|_ |_ |_ |_ |_ |_ |_ |_ |_ 
 _| _| _| _| _| _| _| _| _|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When666666666_ThenItIsParsedAsAmbigous()
    {
        // Arrange
        var expextedResult = "666666666 AMB ['666566666', '686666666']";
        var entry = @"
 _  _  _  _  _  _  _  _  _ 
|_ |_ |_ |_ |_ |_ |_ |_ |_ 
|_||_||_||_||_||_||_||_||_|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When999999999_ThenItIsParsedAsAmbigous()
    {
        // Arrange
        var expextedResult = "999999999 AMB ['899999999', '993999999', '999959999']";
        var entry = @"
 _  _  _  _  _  _  _  _  _ 
|_||_||_||_||_||_||_||_||_|
 _| _| _| _| _| _| _| _| _|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When490067715_ThenItIsParsedAsAmbigous()
    {
        // Arrange
        var expextedResult = "490067715 AMB ['490067115', '490067719', '490867715']";
        var entry = @"
    _  _  _  _  _  _     _ 
|_||_|| || ||_   |  |  ||_ 
  | _||_||_||_|  |  |  | _|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void WhenX23456789_ThenItIsParsed()
    {
        // Arrange
        var expextedResult = "123456789";
        var entry = @"
    _  _     _  _  _  _  _ 
 _| _| _||_||_ |_   ||_||_|
  ||_  _|  | _||_|  ||_| _|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When0X0000051_ThenItIsParsedAs()
    {
        // Arrange
        var expextedResult = "000000051";
        var entry = @"
 _     _  _  _  _  _  _    
| || || || || || || ||_   |
|_||_||_||_||_||_||_| _|  |
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }

    [Fact]
    public void When49086771X_ThenItIsParsed()
    {
        // Arrange
        var expextedResult = "490867715";
        var entry = @"
    _  _  _  _  _  _     _ 
|_||_|| ||_||_   |  |  | _ 
  | _||_||_||_|  |  |  | _|
";
        // Act
        var textResult = Parser.GetParsingResultWithSingleDeviation(entry);

        // Assert
        textResult.Should().Be(expextedResult);
    }
}
