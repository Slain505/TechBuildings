using System;
using System.Collections.Generic;
using UnityEngine.Rendering.UI;

namespace DI
{
    /// <summary>
    /// Represents a Dependency Injection container for managing dependencies.
    /// </summary>
    public class DIContainer
    {
        private readonly DIContainer _parentContainer;
        private readonly Dictionary<(string, Type), DIRegistration> _registrations = new(); 
        private readonly HashSet<(string, Type)> _resolving = new();
        
        /// <summary>
        /// Initialize a new instance of the <see cref="DIContainer"/> class.
        /// </summary>
        /// <param name="parentContainer">The parent container, if any</param>
        public DIContainer(DIContainer parentContainer = null)
        {
            this._parentContainer = parentContainer;
        }
        
        //public void RegisterSingleton<T>(Func<DIContainer, T> factory, bool isSingleton = false) ???

        /// <summary>
        /// Registers a singleton instance of a type without a tag.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="factory">The factory method to create the instance.</param>
        public void RegisterSingleton<T>(Func<DIContainer, T> factory)
        {
            RegisterSingleton(null, factory);
        }
        
        //public void RegisterSingleton<T>(string tag, Func<DIContainer, T> factory, bool isSingleton = false) ???
        
        /// <summary>
        /// Registers a singleton instance of a type with a tag.
        /// </summary>
        /// <param name="tag">The tag to identify the instance.</param>
        /// <param name="factory">The factory method to create the instance.</param>
        /// <typeparam name="T">The type to register.</typeparam>
        public void RegisterSingleton<T>(string tag, Func<DIContainer, T> factory)
        {
            var key = (tag, typeof(T));
            Register(key, factory, true);
        }
        
        /// <summary>
        /// Registers a transient instance of a type without a tag.
        /// </summary>
        /// <param name="factory">The factory method to create the instance.</param>
        /// <typeparam name="T">The type to register.</typeparam>
        public void RegisterTransient<T>(Func<DIContainer, T> factory)
        {
            RegisterTransient(null, factory);
        }
        
        /// <summary>
        ///  Registers a transient instance of a type with a tag.
        /// </summary>
        /// <param name="tag">The tag to identify the instance.</param>
        /// <param name="factory">The factory method to create the instance.</param>
        /// <typeparam name="T">The type to register.</typeparam>
        public void RegisterTransient<T>(string tag, Func<DIContainer, T> factory)
        {
            var key = (tag, typeof(T));
            Register(key, factory, false);
        }
        
        /// <summary>
        /// Registers an existing instance of a type without a tag.
        /// </summary>
        /// <param name="instance">The instance to register.</param>
        /// <typeparam name="T">The type to register.</typeparam>
        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance(null, instance);
        }
        
        /// <summary>
        /// Registers an existing instance of a type with a tag.
        /// </summary>
        /// <param name="tag">The tag to identify the instance.</param>
        /// <param name="instance">The instance to register.</param>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <exception cref="Exception">Thrown when an instance with the same tag and type is already registered.</exception>
        public void RegisterInstance<T>(string tag, T instance)
        {
            var key = (tag, typeof(T));
            if(_registrations.ContainsKey(key))
                throw new Exception($"DI Factory with tag {key.Item1} and type {key.Item2.FullName} already registered");

            _registrations[key] = new DIRegistration
            {
                SingletonInstance = instance,
                IsSingleton = true
            };
        }

        /// <summary>
        /// Resolves an instance of the specified type and tag.
        /// </summary>
        /// <param name="tag">The tag to identify the registration, or null for untagged registrations.</param>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The resolved instance of the specified type.</returns>
        /// <exception cref="Exception">Thrown when a circular dependency is detected or the type cannot be resolved.</exception>
        public T Resolve<T>(string tag = null)
        {
            var key = (tag, typeof(T));

            if (_resolving.Contains(key))
                throw new Exception($"Circular dependency detected for tag {tag} and type {typeof(T).FullName}");
            
            _resolving.Add(key);

            try
            {
                if (_registrations.TryGetValue(key, out var registration))
                {
                    if (!registration.IsSingleton) 
                        return (T)registration.Factory(this);
                
                    if (registration.SingletonInstance == null && registration.Factory != null)
                        registration.SingletonInstance = registration.Factory(this);

                    return (T)registration.SingletonInstance;
                }

                if (_parentContainer != null)
                    return _parentContainer.Resolve<T>(tag);
            }
            finally
            {
                _resolving.Remove(key);
            }
            
            throw new Exception($"Couldn't find DI Factory with tag {tag} and type {typeof(T).FullName}");
        }
        
        /// <summary>
        /// Registers a type with a factory method.
        /// </summary>
        /// <param name="key">The key (tag, type) to identify the registration.</param>
        /// <param name="factory">The factory method to create the instance.</param>
        /// <param name="isSingleton">Indicates whether the instance is a singleton.</param>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <exception cref="Exception">Thrown when a factory with the same tag and type is already registered.</exception>
        private void Register<T>((string, Type) key, Func<DIContainer, T> factory, bool isSingleton)
        {
            if(_registrations.ContainsKey(key))
                throw new Exception($"DI Factory with tag {key.Item1} and type {key.Item2.FullName} already registered");

            _registrations[key] = new DIRegistration
            {
                Factory = container => factory(container),
                IsSingleton = isSingleton
            };
        }
    }
}