using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _commandQueue = new();
    private readonly Thread _workerThread;
    private readonly IScheduler? _scheduler;
    private Action _currentBehavior;
    private bool _isRunning = true;

    public ServerThread(IScheduler? scheduler = null)
    {
        _scheduler = scheduler;
        _currentBehavior = ProcessStandard;
        _workerThread = new Thread(ProcessLoop);
    }

    public void Start() => _workerThread.Start();

    public void EnqueueCommand(ICommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        try
        {
            if (!_commandQueue.IsAddingCompleted)
                _commandQueue.Add(command);
        }
        catch (InvalidOperationException)
        {
        }
    }

    public void WaitUntilFinished(int timeoutMs = Timeout.Infinite)
    {
        _workerThread.Join(timeoutMs);
    }

    public bool IsAlive => _workerThread.IsAlive;

    public void ChangeBehavior(Action newBehavior)
    {
        _currentBehavior = newBehavior;
    }

    public void ExecuteHardStop()
    {
        ValidateExecutionThread();
        _isRunning = false;
    }

    public void ExecuteSoftStop()
    {
        ValidateExecutionThread();
        _commandQueue.CompleteAdding();
        ChangeBehavior(ProcessUntilEmpty);
    }

    private void ProcessLoop()
    {
        while (_isRunning)
        {
            _currentBehavior();
        }
    }

    private void ProcessStandard()
    {
        ICommand? commandToExecute = null;
        try
        {
            if (_scheduler == null)
            {
                commandToExecute = _commandQueue.Take();
                RunCommand(commandToExecute);
            }
            else
            {
                int timeout = _scheduler.HasCommand() ? 0 : Timeout.Infinite;

                if (_commandQueue.TryTake(out var cmd, timeout))
                {
                    commandToExecute = cmd;
                    RunCommand(commandToExecute);
                }
                else if (_scheduler.HasCommand())
                {
                    commandToExecute = _scheduler.Select();
                    RunCommand(commandToExecute);
                }
                else if (_commandQueue.IsCompleted)
                {
                    _isRunning = false;
                }
            }
        }
        catch (InvalidOperationException)
        {
            _isRunning = false;
        }
        catch (Exception ex)
        {
            if (commandToExecute != null)
            {
                ExceptionHandler.OnException?.Invoke(ex, commandToExecute);
            }
        }
    }

    private void ProcessUntilEmpty()
    {
        ICommand? commandToExecute = null;
        try
        {
            if (_commandQueue.TryTake(out var cmd))
            {
                commandToExecute = cmd;
                RunCommand(commandToExecute);
            }
            else if (_scheduler != null && _scheduler.HasCommand())
            {
                commandToExecute = _scheduler.Select();
                RunCommand(commandToExecute);
            }
            else
            {
                _isRunning = false;
            }
        }
        catch (Exception ex)
        {
            if (commandToExecute != null)
            {
                ExceptionHandler.OnException?.Invoke(ex, commandToExecute);
            }
        }
    }

    private void RunCommand(ICommand cmd)
    {
        cmd.Execute();

        if (_scheduler != null && cmd is ILongCommand longCmd && !longCmd.IsCompleted)
        {
            _scheduler.Add(cmd);
        }
    }

    private void ValidateExecutionThread()
    {
        if (Thread.CurrentThread.ManagedThreadId != _workerThread.ManagedThreadId)
        {
            throw new InvalidOperationException("Операция остановки должна быть выполнена внутри ServerThread.");
        }
    }
}
