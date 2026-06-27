using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05
{
    public class ClassAnalyzer
    {
        private Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type;
        }

        public IEnumerable<string> GetPublicMethods()
        {
            return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                        .Select(m => m.Name);
        }

        public IEnumerable<string> GetMethodParams(string methodname)
        {
            var method = _type.GetMethod(methodname);
            if (method == null) return Enumerable.Empty<string>();
            var returnType = new[] { method.ReturnType.Name };
            var paramNames = method.GetParameters().Select(p => p.Name!);
            return returnType.Concat(paramNames);
        }

        public IEnumerable<string> GetAllFields()
        {
            return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Select(f => f.Name);
        }

        public IEnumerable<string> GetProperties()
        {
            return _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Select(p => p.Name);
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return _type.IsDefined(typeof(T), false);
        }
    }
}
