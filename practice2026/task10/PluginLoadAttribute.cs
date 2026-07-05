using System;

namespace task10;

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public string[] Dependencies { get; set; } = Array.Empty<string>();
}
