using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ScottPlot;
using Xunit;
using task17;

namespace task18tests;

public class SchedulerTests
{
    private const int TimeoutThresholdMs = 3000;
    private const int PlotWidth = 800;
    private const int PlotHeight = 600;

    private class SegmentedTask : ILongCommand
    {
        private readonly string _identifier;
        private readonly int _targetSegments;
        private readonly List<string> _executionTrace;
        private int _segmentsDone = 0;

        public SegmentedTask(string identifier, int targetSegments, List<string> executionTrace)
        {
            _identifier = identifier;
            _targetSegments = targetSegments;
            _executionTrace = executionTrace;
        }

        public bool IsCompleted => _segmentsDone >= _targetSegments;

        public void Execute()
        {
            if (IsCompleted) return;

            _segmentsDone++;
            lock (_executionTrace)
            {
                _executionTrace.Add($"{_identifier}[{_segmentsDone}]");
            }
        }
    }

    private class VisualTask : ILongCommand
    {
        private readonly int _requiredIterations;
        private readonly int _sleepDuration;
        private readonly Stopwatch _globalTimer;
        private int _currentIteration = 0;

        public List<double> TimeStamps { get; } = new();
        public List<double> ProgressValues { get; } = new();

        public VisualTask(int requiredIterations, int sleepDuration, Stopwatch globalTimer)
        {
            _requiredIterations = requiredIterations;
            _sleepDuration = sleepDuration;
            _globalTimer = globalTimer;

            TimeStamps.Add(0);
            ProgressValues.Add(0);
        }

        public bool IsCompleted => _currentIteration >= _requiredIterations;

        public void Execute()
        {
            if (IsCompleted) return;

            _currentIteration++;
            Thread.Sleep(_sleepDuration);

            TimeStamps.Add(_globalTimer.Elapsed.TotalMilliseconds);
            ProgressValues.Add((double)_currentIteration / _requiredIterations * 100.0);
        }
    }

    [Fact]
    public void Scheduler_DistributesExecution_Equally()
    {
        var trace = new List<string>();
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        server.EnqueueCommand(new SegmentedTask("TaskX", 3, trace));
        server.EnqueueCommand(new SegmentedTask("TaskY", 2, trace));
        server.EnqueueCommand(new SoftStopCommand(server));

        server.Start();
        server.WaitUntilFinished(TimeoutThresholdMs);

        var expectedTrace = new[] { "TaskX[1]", "TaskY[1]", "TaskX[2]", "TaskY[2]", "TaskX[3]" };
        Assert.Equal(expectedTrace, trace);
    }

    [Fact]
    public void HardStop_Prevents_ScheduledTasksExecution()
    {
        var trace = new List<string>();
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        server.EnqueueCommand(new SegmentedTask("TaskX", 8, trace));
        server.EnqueueCommand(new SegmentedTask("TaskY", 8, trace));
        server.EnqueueCommand(new HardStopCommand(server));

        server.Start();
        server.WaitUntilFinished(TimeoutThresholdMs);

        Assert.Contains("TaskX[1]", trace);
        Assert.Contains("TaskY[1]", trace);
        Assert.DoesNotContain("TaskX[8]", trace);
    }

    [Fact]
    public void BuildPerformanceReport_AndChart()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        var timer = Stopwatch.StartNew();

        var job1 = new VisualTask(3, 10, timer);
        var job2 = new VisualTask(4, 15, timer);
        var job3 = new VisualTask(2, 20, timer);

        server.EnqueueCommand(job1);
        server.EnqueueCommand(job2);
        server.EnqueueCommand(job3);
        server.EnqueueCommand(new SoftStopCommand(server));

        server.Start();
        server.WaitUntilFinished(TimeoutThresholdMs);

        var myPlot = new Plot();
        myPlot.Title("Динамика выполнения потоковых задач (Round Robin)");
        myPlot.XLabel("Затраченное время (мс)");
        myPlot.YLabel("Общий прогресс (%)");

        var line1 = myPlot.Add.Scatter(job1.TimeStamps.ToArray(), job1.ProgressValues.ToArray());
        line1.LegendText = "Поток 1";
        line1.LineWidth = 2.5f;
        line1.MarkerSize = 6;

        var line2 = myPlot.Add.Scatter(job2.TimeStamps.ToArray(), job2.ProgressValues.ToArray());
        line2.LegendText = "Поток 2";
        line2.LineWidth = 2.5f;
        line2.MarkerSize = 6;

        var line3 = myPlot.Add.Scatter(job3.TimeStamps.ToArray(), job3.ProgressValues.ToArray());
        line3.LegendText = "Поток 3";
        line3.LineWidth = 2.5f;
        line3.MarkerSize = 6;

        myPlot.ShowLegend();

        string rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        myPlot.SavePng(Path.Combine(rootDirectory, "progress_chart.png"), PlotWidth, PlotHeight);

        using var textWriter = new StreamWriter(Path.Combine(rootDirectory, "report.txt"));
        textWriter.WriteLine($"Время завершения Потока 1: {job1.TimeStamps.Last():F4} мс");
        textWriter.WriteLine($"Время завершения Потока 2: {job2.TimeStamps.Last():F4} мс");
        textWriter.WriteLine($"Время завершения Потока 3: {job3.TimeStamps.Last():F4} мс");
    }
}
