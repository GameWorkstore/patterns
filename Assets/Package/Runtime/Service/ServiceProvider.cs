using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        private static readonly ServicePriorityComparer _serviceSorter = new ServicePriorityComparer();

        /// <summary>
        /// Gets the first service that is T.
        /// </summary>
        /// <typeparam name="T">Any IService</typeparam>
        /// <returns></returns>
        public static T GetAbstract<T>() where T : IService
        {
            foreach(var pair in _services)
            {
                if(pair.Value is T)
                {
                    return pair.Value as T;
                }
            }
            Debug.LogError("[ServiceProvider]: No concrete service initialized for this abstraction.");
            return null;
        }

        /// <summary>
        /// Get or Create T service. must be a concrete class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : IService, new()
        {
            if (_services.TryGetValue(typeof(T), out IService service))
            {
                return service as T;
            }
            var nService = new T();
            if(nService == null)
            {
                Debug.LogError("[ServiceProvider]: Error while creating " + typeof(T).Name + " service.");
                return null;
            }
            nService.Preprocess();
            _services.Add(typeof(T),nService);
            return nService;
        }

        public static bool RemoveService<T>() where T : IService
        {
            Type type = typeof(T);
            if (!_services.ContainsKey(type)) return false;
            IService service = _services[type];
            service.Postprocess();
            _services.Remove(type);
            return true;
        }

        /// <summary>
        /// Destroy and clear all services.
        /// </summary>
        public static void Shutdown()
        {
            List<IService> services = _services.Values.ToList();
            services.Sort(_serviceSorter);
            foreach (IService service in services)
            {
                service.Postprocess();
            }
            _services.Clear();
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