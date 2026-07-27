using System;
using System.Collections.Generic;

namespace Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();
        public static void Register<T>(T service) => _services[typeof(T)] = service;
        public static T Get<T>() => (T)_services[typeof(T)];
        public static void Clear() => _services.Clear();
    }
}
