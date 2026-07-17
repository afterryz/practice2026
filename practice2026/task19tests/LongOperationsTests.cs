using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using ScottPlot;
using task17;

namespace task19tests;

public class LongOperationsTests
{
    private const int ThreadJoinTimeoutMs = 3000;
    private const int ImageWidth = 800;
    private const int ImageHeight = 600;

    [Fact]
    public void IllustrateFiveTestCommands()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        var log = new List<int>();

        var cmd1 = new TestCommand(1, log);
        var cmd2 = new TestCommand(2, log);
        var cmd3 = new TestCommand(3, log);
        var cmd4 = new TestCommand(4, log);
        var cmd5 = new TestCommand(5, log);

        server.EnqueueCommand(cmd1);
        server.EnqueueCommand(cmd2);
        server.EnqueueCommand(cmd3);
        server.EnqueueCommand(cmd4);
        server.EnqueueCommand(cmd5);

        server.Start();

        Thread.Sleep(300);

        var hardStop = new HardStopCommand(server);
        server.EnqueueCommand(hardStop);

        server.WaitUntilFinished(ThreadJoinTimeoutMs);
        Assert.False(server.IsAlive);

        Assert.Equal(15, log.Count);
        for (int id = 1; id <= 5; id++)
        {
            int count = log.Count(x => x == id);
            Assert.Equal(3, count);
        }

        var plot = new Plot();
        plot.Title("Сетка выполнения задач (Round Robin)");
        plot.XLabel("Сквозной шаг выполнения сервера");
        plot.YLabel("ID задачи");

        for (int id = 1; id <= 5; id++)
        {
            var taskSteps = new List<double>();
            for (int i = 0; i < log.Count; i++)
            {
                if (log[i] == id)
                {
                    taskSteps.Add(i + 1);
                }
            }

            double[] xs = taskSteps.ToArray();
            double[] ys = Enumerable.Repeat((double)id, xs.Length).ToArray();

            var scatter = plot.Add.Scatter(xs, ys);
            scatter.LineWidth = 2;
            scatter.MarkerSize = 12;
            scatter.LegendText = $"Задача {id}";
        }

        plot.ShowLegend();
        plot.Axes.SetLimits(0.5, 15.5, 0.5, 5.5);

        string rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        string filePathPNG = Path.Combine(rootDirectory, "progress_chart.png");
        string filePathTXT = Path.Combine(rootDirectory, "report.txt");

        plot.SavePng(filePathPNG, ImageWidth, ImageHeight);

        using (var writer = new StreamWriter(filePathTXT))
        {
            writer.WriteLine("Отчет по выполнению длительных операций (Задание 19):");
            writer.WriteLine("Шулепко Никита Александрович");
            writer.WriteLine();
            writer.WriteLine("Порядок выполнения шагов:");
            for (int i = 0; i < log.Count; i++)
            {
                writer.WriteLine($"Шаг {i + 1}: Выполнена задача {log[i]}");
            }
        }
    }
}
