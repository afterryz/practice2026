using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task10;

public class PluginLoader
{
    public List<Type> DiscoveredPlugins { get; } = new();

    public void LoadPluginsFromDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        string[] dllFiles = Directory.GetFiles(directoryPath, "*.dll");

        foreach (string file in dllFiles)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(file);
                IEnumerable<Type> validTypes = assembly.GetTypes()
                    .Where(t => t.IsClass
                             && !t.IsAbstract
                             && typeof(IPlugin).IsAssignableFrom(t)
                             && t.GetCustomAttribute<PluginLoadAttribute>() != null);

                DiscoveredPlugins.AddRange(validTypes);
            }
            catch
            {
                continue;
            }
        }
    }

    public void RunAll()
    {
        List<Type> sortedTypes = GetTopologicalSort();

        foreach (Type type in sortedTypes)
        {
            IPlugin pluginInstance = (IPlugin)Activator.CreateInstance(type)!;
            pluginInstance.Execute();
        }
    }

    private List<Type> GetTopologicalSort()
    {
        var sortedList = new List<Type>();
        var visited = new HashSet<Type>();
        var processing = new HashSet<Type>();

        void Dfs(Type currentType)
        {
            if (visited.Contains(currentType))
            {
                return;
            }

            if (processing.Contains(currentType))
            {
                throw new InvalidOperationException("Circular dependency detected.");
            }

            processing.Add(currentType);

            var attribute = currentType.GetCustomAttribute<PluginLoadAttribute>();
            string[] dependencies = attribute?.Dependencies ?? Array.Empty<string>();

            foreach (string depName in dependencies)
            {
                Type dependentType = DiscoveredPlugins.FirstOrDefault(t => t.Name == depName);
                if (dependentType != null)
                {
                    Dfs(dependentType);
                }
            }

            processing.Remove(currentType);
            visited.Add(currentType);
            sortedList.Add(currentType);
        }

        foreach (Type plugin in DiscoveredPlugins)
        {
            if (!visited.Contains(plugin))
            {
                Dfs(plugin);
            }
        }

        return sortedList;
    }
}
