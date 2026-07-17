using System;

namespace task17;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _targetThread;

    public HardStopCommand(ServerThread targetThread)
    {
        _targetThread = targetThread ?? throw new ArgumentNullException(nameof(targetThread));
    }

    public void Execute()
    {
        _targetThread.ExecuteHardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _targetThread;

    public SoftStopCommand(ServerThread targetThread)
    {
        _targetThread = targetThread ?? throw new ArgumentNullException(nameof(targetThread));
    }

    public void Execute()
    {
        _targetThread.ExecuteSoftStop();
    }
}
