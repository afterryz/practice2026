using System;

namespace task14;

public class OneThreadIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step)
    {
        int intervals = (int)Math.Round((b - a) / step);
        if (intervals < 1) intervals = 1;

        double actualStep = (b - a) / intervals;
        double total = 0.0;

        for (int k = 0; k < intervals; k++)
        {
            double left = a + k * actualStep;
            double right = a + (k + 1) * actualStep;
            total += (function(left) + function(right)) / 2.0 * actualStep;
        }

        return total;
    }
}
