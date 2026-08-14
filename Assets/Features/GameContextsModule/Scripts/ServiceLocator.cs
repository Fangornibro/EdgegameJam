using System;
using System.Collections.Generic;

namespace Features.GameContextsModule.Scripts {
    public static class ServiceLocator {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<TService>(TService service) where TService : class {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (!_services.TryAdd(typeof(TService), service))
                throw new InvalidOperationException($"Service {typeof(TService).Name} is already registered.");
        }

        public static TService Get<TService>() where TService : class {
            if (!_services.TryGetValue(typeof(TService), out object service))
                throw new InvalidOperationException($"Service {typeof(TService).Name} is not registered.");

            return (TService)service;
        }

        public static bool TryGet<TService>(out TService service) where TService : class {
            if (_services.TryGetValue(typeof(TService), out object registered)) {
                service = (TService)registered;
                return true;
            }

            service = null;
            return false;
        }

        public static bool Unregister<TService>() where TService : class =>
            _services.Remove(typeof(TService));

        public static void Clear() =>
            _services.Clear();
    }
}
