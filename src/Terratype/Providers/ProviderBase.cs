﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Terratype.Providers
{
    [DebuggerDisplay("{Name}")]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ProviderBase
    {
        /// <summary>
        /// Unique identifier of provider
        /// </summary>
        [JsonProperty]
        public abstract string Id { get; }

        /// <summary>
        /// Name of provider
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Description of provider
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Url that a developer can use to get more information about this map provider
        /// </summary>
        public abstract string ReferenceUrl { get; }

        /// <summary>
        /// Coordinate systems that this map provider can handle
        /// </summary>
        public abstract IDictionary<string, Type> CoordinateSystems { get; }

        /// <summary>
        /// Can this map handle searches
        /// </summary>
        public virtual bool CanSearch
        {
            get
            {
                return false;
            }
        }

        //  Register all derived classes
        private static readonly Lazy<Dictionary<string, Type>> providers =
            new Lazy<Dictionary<string, Type>>(() =>
            {
                Dictionary<string, Type> installed = new Dictionary<string, Type>();
                Assembly currAssembly = Assembly.GetExecutingAssembly();

                Type baseType = typeof(ProviderBase);

                foreach (Type type in currAssembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract ||
                        !type.IsSubclassOf(baseType))
                    {
                        continue;
                    }

                    var derivedObject = System.Activator.CreateInstance(type) as ProviderBase;
                    if (derivedObject != null)
                    {
                        installed.Add(derivedObject.Id, derivedObject.GetType());
                    }
                }

                return installed;
            });

        public static Dictionary<string, Type> Providers
        {
            get
            {
                return providers.Value;
            }
        }

        /// <summary>
        /// Create a derived Provider with a particular Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProviderBase Create(string id)
        {
            Type derivedType = null;
            if (Providers.TryGetValue(id, out derivedType))
            {
                return System.Activator.CreateInstance(derivedType) as ProviderBase;
            }
            return null;
        }

        /// <summary>
        /// Create a derived Provider from a particular type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProviderBase Create(Type type)
        {
            return System.Activator.CreateInstance(type) as ProviderBase;
        }

    }
}