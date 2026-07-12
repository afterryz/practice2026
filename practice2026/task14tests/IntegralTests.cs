using System;
using Xunit;
using task14;

namespace task14tests;

public class IntegralTests
{
    [Fact]
    public void CheckLinearFunction_Symmetric_IsZero()
    {
        Func<double, double> func = x => x;
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, func, 1e-4, 2), 1e-4);
    }

    [Fact]
    public void CheckSinFunction_Symmetric_IsZero()
    {
        Func<double, double> func = Math.Sin;
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, func, 1e-5, 8), 1e-4);
    }

    [Fact]
    public void CheckLinearFunction_Positive_IsCorrect()
    {
        Func<double, double> func = x => x;
        Assert.Equal(12.5, DefiniteIntegral.Solve(0, 5, func, 1e-6, 8), 1e-5);
    }
}
