using System;

namespace task17;

public static class ExceptionHandler
{
    public static Action<Exception, ICommand>? OnException { get; set; }
}
