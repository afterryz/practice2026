using System;
using System.Collections.Generic;

namespace task17;

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _taskQueue = new();

    public bool HasCommand()
    {
        return _taskQueue.Count > 0;
    }

    public ICommand Select()
    {
        if (_taskQueue.Count == 0)
        {
            throw new InvalidOperationException();
        }

        return _taskQueue.Dequeue();
    }

    public void Add(ICommand cmd)
    {
        if (cmd == null)
        {
            throw new ArgumentNullException(nameof(cmd));
        }

        _taskQueue.Enqueue(cmd);
    }
}
