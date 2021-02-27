using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    public static class ServiceProvider
    {
        private static Dictionary<Type, IService> _serviceDictionary = new Dictionary<Type, IService>();
        private static ServicePriorityComparer _serviceSort = new ServicePriorityComparer();

        public static T GetService<T>() where T : IService, new()
        {
            Type type = typeof(T);
            if (_serviceDictionary.TryGetValue(type, out IService service))
            {
                return service as T;
            }
            if (type.IsInterface)
            {
                Debug.LogError("I cant provide you with a interface service.");
                return null;
            }
            if (type.IsAbstract)
            {
                Debug.LogError("I cant provide you with a abstract service.");
                return null;
            }
            service = new T();
            if(service == null)
            {
                Debug.LogError("Error while trying to create the service.");
                return null;
            }
            service.Preprocess();
            _serviceDictionary[type] = service;
            return service as T;
        }

        public static bool RemoveService<T>() where T : IService
        {
            Type type = typeof(T);
            if (!_serviceDictionary.ContainsKey(type)) return false;
            IService service = _serviceDictionary[type];
            service.Postprocess();
            _serviceDictionary.Remove(type);
            return true;
        }

        public static void ShutdownServices()
        {
            List<IService> services = _serviceDictionary.Values.ToList();
            services.Sort(_serviceSort);
            foreach (IService service in services)
            {
                service.Postprocess();
            }
        }

        public class ServicePriorityComparer : IComparer<IService>
        {
            public int Compare(IService x, IService y)
            {
                return x.Priority().CompareTo(y.Priority());
            }
        }
    }
}