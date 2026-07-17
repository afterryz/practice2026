using System;
using Xunit;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    private const int WaitTimeoutMs = 3000;

    private class SpyCommand : ICommand
    {
        public bool IsExecuted { get; private set; }
        private readonly Action? _executionLogic;

        public SpyCommand(Action? executionLogic = null)
        {
            _executionLogic = executionLogic;
        }

        public void Execute()
        {
            IsExecuted = true;
            _executionLogic?.Invoke();
        }
    }

    [Fact]
    public void HardStop_TerminatesImmediately_IgnoresRemainingCommands()
    {
        var server = new ServerThread();
        var firstCommand = new SpyCommand();
        var stopCommand = new HardStopCommand(server);
        var ignoredCommand = new SpyCommand();

        server.EnqueueCommand(firstCommand);
        server.EnqueueCommand(stopCommand);
        server.EnqueueCommand(ignoredCommand);

        server.Start();
        server.WaitUntilFinished(WaitTimeoutMs);

        Assert.True(firstCommand.IsExecuted);
        Assert.False(ignoredCommand.IsExecuted);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void SoftStop_ProcessesRemainingCommands_BeforeTermination()
    {
        var server = new ServerThread();
        var firstCommand = new SpyCommand();
        var stopCommand = new SoftStopCommand(server);
        var lastCommand = new SpyCommand();

        server.EnqueueCommand(firstCommand);
        server.EnqueueCommand(stopCommand);
        server.EnqueueCommand(lastCommand);

        server.Start();
        server.WaitUntilFinished(WaitTimeoutMs);

        Assert.True(firstCommand.IsExecuted);
        Assert.True(lastCommand.IsExecuted);
        Assert.False(server.IsAlive);
    }

    [Fact]
    public void StopCommands_ExecutedOnWrongThread_ThrowsException()
    {
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);
        var softStop = new SoftStopCommand(server);

        var exHard = Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Equal("Операция остановки должна быть выполнена внутри ServerThread.", exHard.Message);

        var exSoft = Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        Assert.Equal("Операция остановки должна быть выполнена внутри ServerThread.", exSoft.Message);
    }

    [Fact]
    public void CommandException_IsCaught_AndSentToHandler()
    {
        var server = new ServerThread();
        var expectedException = new ArgumentException("Test Error");
        var faultingCommand = new SpyCommand(() => throw expectedException);
        var hardStop = new HardStopCommand(server);

        Exception? capturedException = null;
        ICommand? capturedCommand = null;

        ExceptionHandler.OnException = (ex, cmd) =>
        {
            capturedException = ex;
            capturedCommand = cmd;
        };

        server.EnqueueCommand(faultingCommand);
        server.EnqueueCommand(hardStop);

        server.Start();
        server.WaitUntilFinished(WaitTimeoutMs);

        Assert.Same(expectedException, capturedException);
        Assert.Same(faultingCommand, capturedCommand);
    }
}
