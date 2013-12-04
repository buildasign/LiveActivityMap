using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using LiveActivityMap.Services;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace LiveActivityMap.IoC
{
    public class StructureMapSetup : IBootstrapper
    {
        protected static bool _hasBeenInitalized = false;

        protected static readonly object Locker = new object();
        /// <summary>
        /// This calls the boostrapper on StructureMap.
        /// </summary>
        public static void EnsureStructureMapIsReady()
        {
            if (_hasBeenInitalized)
                return;

            lock (Locker)
            {
                if (_hasBeenInitalized)
                    return;

                new StructureMapSetup().BootstrapStructureMap();
            }

            _hasBeenInitalized = true;
        }

        public void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(initalizer =>
            {
                initalizer.Scan(scanner =>
                {
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                    scanner.LookForRegistries();
                });

                initalizer.For<IGeocoder>()
                    .Use<MapquestGeocoder>()
                    .Ctor<string>("apikey")
                    .Is(ConfigurationManager.AppSettings["MapquestAPIKey"]);

                initalizer.IgnoreStructureMapConfig = true;
            });

            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(ObjectFactory.Container));
        }
    }
}