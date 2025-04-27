using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (!services.ContainsKey(type))
            {
                services[type] = service;
            }
            else
            {
                throw new InvalidOperationException($"Service of type {type.Name} is already registered.");
            }
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                return (T)services[type];
            }
            else
            {
                throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
            }
        }

        public static void Unregister<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(typeof(T));
            }
            else
            {
                throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
            }


        }
    }
}