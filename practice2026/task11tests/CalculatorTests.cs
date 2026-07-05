using System;
using Xunit;
using task11;

namespace task11tests;

public class CalculatorTests
{
    private readonly ICalculator _calculator;

    public CalculatorTests()
    {
        _calculator = CalculatorFactory.BuildDynamicCalculator();
    }

    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(-9, -3, -12)]
    [InlineData(0, 0, 0)]
    public void Add_ShouldReturnCorrectSum(int a, int b, int expected)
    {
        Assert.Equal(expected, _calculator.Add(a, b));
    }

    [Theory]
    [InlineData(10, 9, 1)]
    [InlineData(-7, -9, 2)]
    public void Minus_ShouldReturnCorrectDifference(int a, int b, int expected)
    {
        Assert.Equal(expected, _calculator.Minus(a, b));
    }

    [Theory]
    [InlineData(7, 3, 21)]
    [InlineData(-12, 3, -36)]
    public void Mul_ShouldReturnCorrectProduct(int a, int b, int expected)
    {
        Assert.Equal(expected, _calculator.Mul(a, b));
    }

    [Theory]
    [InlineData(8, 2, 4)]
    [InlineData(-6, -6, 1)]
    public void Div_ShouldReturnCorrectQuotient(int a, int b, int expected)
    {
        Assert.Equal(expected, _calculator.Div(a, b));
    }

    [Fact]
    public void Div_ByZero_ShouldThrowDivideByZeroException()
    {
        Assert.Throws<DivideByZeroException>(() => _calculator.Div(10, 0));
    }
}
