using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0)
        {
            throw new ArgumentException("Thread count must be greater than zero.", nameof(threadsNumber));
        }

        if (step <= 0)
        {
            throw new ArgumentException("Step size must be greater than zero.", nameof(step));
        }

        bool isReversed = false;
        if (a > b)
        {
            (a, b) = (b, a);
            isReversed = true;
        }

        double totalSum = 0.0;
        double intervalLength = b - a;

        if (intervalLength == 0)
        {
            return 0.0;
        }

        using var barrier = new Barrier(threadsNumber + 1);
        double subIntervalLength = intervalLength / threadsNumber;
        Thread[] threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int index = i;
            threads[i] = new Thread(() =>
            {
                double localA = a + index * subIntervalLength;
                double localB = localA + subIntervalLength;

                int stepsCount = (int)Math.Ceiling((localB - localA) / step);
                double currentStep = (localB - localA) / stepsCount;

                double localSum = 0.5 * (function(localA) + function(localB));

                for (int j = 1; j < stepsCount; j++)
                {
                    double x = localA + j * currentStep;
                    localSum += function(x);
                }

                localSum *= currentStep;

                double initialValue;
                double computedValue;
                do
                {
                    initialValue = totalSum;
                    computedValue = initialValue + localSum;
                }
                while (Interlocked.CompareExchange(ref totalSum, computedValue, initialValue) != initialValue);

                barrier.SignalAndWait();
            });

            threads[i].Start();
        }

        barrier.SignalAndWait();

        return isReversed ? -totalSum : totalSum;
    }
}
