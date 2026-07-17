using System;
using System.Collections.Generic;

namespace task17;

public class TestCommand : ILongCommand
{
    private readonly int _id;
    private int _counter = 0;
    private readonly List<int> _executionOrder;

    public TestCommand(int id, List<int> executionOrder)
    {
        _id = id;
        _executionOrder = executionOrder;
    }

    public bool IsCompleted => _counter >= 3;

    public void Execute()
    {
        if (IsCompleted) return;

        _counter++;
        Console.WriteLine($"Поток {_id} вызов {_counter}");

        lock (_executionOrder)
        {
            _executionOrder.Add(_id);
        }
    }
}
