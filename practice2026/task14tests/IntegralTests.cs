using System;
using Xunit;
using task14;

namespace task14tests;

public class IntegralTests
{
    [Fact]
    public void Solve_LinearFunctionSymmetricInterval_ReturnsZero()
    {
        Func<double, double> linear = x => x;
        double result = DefiniteIntegral.Solve(-1.0, 1.0, linear, 1e-4, 2);
        Assert.Equal(0.0, result, 1e-4);
    }

    [Fact]
    public void Solve_LinearFunctionPositiveInterval_ReturnsCorrectArea()
    {
        Func<double, double> linear = x => x;
        double result = DefiniteIntegral.Solve(0.0, 5.0, linear, 1e-6, 8);
        Assert.Equal(12.5, result, 1e-5);
    }

    [Fact]
    public void Solve_SineFunctionSymmetricInterval_ReturnsZero()
    {
        Func<double, double> sine = Math.Sin;
        double result = DefiniteIntegral.Solve(-1.0, 1.0, sine, 1e-5, 8);
        Assert.Equal(0.0, result, 1e-4);
    }

    [Fact]
    public void Solve_LogarithmicFunction_ReturnsCorrectArea()
    {
        Func<double, double> log = Math.Log;
        double result = DefiniteIntegral.Solve(1.0, Math.E, log, 1e-5, 4);
        Assert.Equal(1.0, result, 1e-4);
    }

    [Fact]
    public void Solve_ConstantFunction_ReturnsCorrectArea()
    {
        Func<double, double> constant = x => 5.0;
        double result = DefiniteIntegral.Solve(0.0, 2.0, constant, 1e-4, 2);
        Assert.Equal(10.0, result, 1e-4);
    }

    [Fact]
    public void Solve_ReversedInterval_ReturnsNegativeValue()
    {
        Func<double, double> quadratic = x => x * x;
        double result = DefiniteIntegral.Solve(-1.0, -2.0, quadratic, 1e-5, 4);
        Assert.Equal(-7.0 / 3.0, result, 1e-4);
    }
}
