using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0) throw new ArgumentException("Thread count must be > 0");

        double resultSum = 0.0;
        double fullLength = b - a;
        double chunkWidth = fullLength / threadsNumber;

        using (var syncBarrier = new Barrier(threadsNumber + 1))
        {
            Thread[] workers = new Thread[threadsNumber];

            for (int i = 0; i < threadsNumber; i++)
            {
                int id = i;
                workers[i] = new Thread(() =>
                {
                    double startX = a + id * chunkWidth;
                    double endX = a + (id + 1) * chunkWidth;

                    int intervals = (int)Math.Round((endX - startX) / step);
                    if (intervals < 1) intervals = 1;

                    double actualStep = (endX - startX) / intervals;
                    double localAccumulator = 0.0;

                    for (int k = 0; k < intervals; k++)
                    {
                        double left = startX + k * actualStep;
                        double right = startX + (k + 1) * actualStep;
                        localAccumulator += (function(left) + function(right)) / 2.0 * actualStep;
                    }
                    
                    double current, next;
                    do
                    {
                        current = resultSum;
                        next = current + localAccumulator;
                    }
                    while (Interlocked.CompareExchange(ref resultSum, next, current) != current);

                    syncBarrier.SignalAndWait();
                });
                workers[i].Start();
            }
            syncBarrier.SignalAndWait();
        }
        return resultSum;
    }
}
