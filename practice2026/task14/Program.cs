using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScottPlot;

namespace task14;

class Program
{
    static readonly Func<double, double> TargetFunction = Math.Sin;
    const double StartBorder = -100.0;
    const double EndBorder = 100.0;
    const double ExpectedResult = 0.0;
    const double Precision = 1e-4;
    const int TestIterations = 8; 

    static void Main()
    {
        Console.WriteLine("--- ЭТАП 1: Анализ шагов интегрирования ---");
        double[] stepVariants = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        double bestStep = 0;
        double minTime = double.MaxValue;

        foreach (var s in stepVariants)
        {
            double value = OneThreadIntegral.Solve(StartBorder, EndBorder, TargetFunction, s);
            double diff = Math.Abs(value - ExpectedResult);
            bool isOk = diff <= Precision;
            double time = MeasureSequence(s);

            Console.WriteLine($"Шаг: {s:E1} | Значение: {value,15:F10} | Ошибка: {diff:E4} | Время: {time:F2} мс | Успех: {isOk}");

            if (isOk && time < minTime)
            {
                bestStep = s;
                minTime = time;
            }
        }

        Console.WriteLine($"\nОптимальный шаг найден: {bestStep:E1}");
        Console.WriteLine("Для демонстрации преимущества потоков используем тяжелую нагрузку (шаг 1e-6).\n");

        double heavyStep = 1e-6;
        int threadLimit = Environment.ProcessorCount * 2;

        var tCounts = new List<int>();
        var execTimes = new List<double>();
        double minMultiTime = double.MaxValue;
        int bestThreadCount = 1;

        Console.WriteLine("--- ЭТАП 2: Замеры многопоточности ---");
        for (int threads = 1; threads <= threadLimit; threads++)
        {
            double time = MeasureParallel(heavyStep, threads);
            tCounts.Add(threads);
            execTimes.Add(time);

            Console.WriteLine($"Потоки: {threads,2} -> Время: {time:F2} мс");

            if (time < minMultiTime)
            {
                minMultiTime = time;
                bestThreadCount = threads;
            }
        }

        double singleTime = MeasureSequence(heavyStep);
        double speedupPercentage = ((singleTime - minMultiTime) / singleTime) * 100.0;

        Console.WriteLine("\n--- ИТОГОВЫЕ РЕЗУЛЬТАТЫ ---");
        Console.WriteLine($"Однопоточно: {singleTime:F2} мс");
        Console.WriteLine($"Многопоточно ({bestThreadCount} потоков): {minMultiTime:F2} мс");
        Console.WriteLine($"Процент ускорения: {speedupPercentage:F2}%");

        var chart = new Plot();
        var scatter = chart.Add.Scatter(execTimes.ToArray(), tCounts.Select(x => (double)x).ToArray());
        scatter.Color = Colors.Green;
        scatter.LineWidth = 3;
        scatter.MarkerShape = MarkerShape.FilledSquare;
        scatter.MarkerSize = 7;

        chart.Title("Анализ производительности многопоточного вычисления");
        chart.XLabel("Затраченное время (мс)");
        chart.YLabel("Число потоков");
        chart.SavePng("result_on_graphic.png", 800, 600);

        using (var sw = new StreamWriter("results_in_text.txt"))
        {
            sw.WriteLine($"Рабочий шаг интегрирования: {heavyStep:E1}");
            sw.WriteLine($"Оптимальное количество потоков: {bestThreadCount}");
            sw.WriteLine($"Время однопоточного выполнения: {singleTime:F2} мс");
            sw.WriteLine($"Время многопоточного выполнения: {minMultiTime:F2} мс");
            sw.WriteLine($"Ускорение алгоритма: {speedupPercentage:F2}%");
        }
        Console.WriteLine("\nФайлы result_on_graphic.png и results_in_text.txt успешно созданы.");
    }

    static double MeasureSequence(double step)
    {
        double total = 0;
        for (int i = 0; i < TestIterations; i++)
        {
            var sw = Stopwatch.StartNew();
            OneThreadIntegral.Solve(StartBorder, EndBorder, TargetFunction, step);
            sw.Stop();
            total += sw.Elapsed.TotalMilliseconds;
        }
        return total / TestIterations;
    }

    static double MeasureParallel(double step, int threads)
    {
        double total = 0;
        for (int i = 0; i < TestIterations; i++)
        {
            var sw = Stopwatch.StartNew();
            DefiniteIntegral.Solve(StartBorder, EndBorder, TargetFunction, step, threads);
            sw.Stop();
            total += sw.Elapsed.TotalMilliseconds;
        }
        return total / TestIterations;
    }
}
