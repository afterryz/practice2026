using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task10;

namespace task10tests;

public static class TestExecutionTracker
{
    public static List<string> ExecutionOrder = new();
}

[PluginLoad]
public class AlphaPlugin : IPlugin
{
    public void Execute()
    {
        TestExecutionTracker.ExecutionOrder.Add("Alpha");
    }
}

[PluginLoad(Dependencies = new[] { "AlphaPlugin" })]
public class BetaPlugin : IPlugin
{
    public void Execute()
    {
        TestExecutionTracker.ExecutionOrder.Add("Beta");
    }
}

[PluginLoad(Dependencies = new[] { "AlphaPlugin", "BetaPlugin" })]
public class GammaPlugin : IPlugin
{
    public void Execute()
    {
        TestExecutionTracker.ExecutionOrder.Add("Gamma");
    }
}

public class PluginSystemTests : IDisposable
{
    private readonly string _tempDirectory;

    public PluginSystemTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"Task10Tests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    [Fact]
    public void Loader_ExecutesPlugins_InCorrectTopologicalOrder()
    {
        TestExecutionTracker.ExecutionOrder.Clear();

        var loader = new PluginLoader();
        string testAssemblyPath = Path.GetDirectoryName(typeof(PluginSystemTests).Assembly.Location);

        loader.LoadPluginsFromDirectory(testAssemblyPath);
        loader.RunAll();

        Assert.Equal(new[] { "Alpha", "Beta", "Gamma" }, TestExecutionTracker.ExecutionOrder);
    }

    [Fact]
    public void Loader_HandlesEmptyDirectory_WithoutExceptions()
    {
        var loader = new PluginLoader();
        loader.LoadPluginsFromDirectory(_tempDirectory);
        loader.RunAll();

        Assert.Empty(loader.DiscoveredPlugins);
    }
}
