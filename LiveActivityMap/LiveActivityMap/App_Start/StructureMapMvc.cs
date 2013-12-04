using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveActivityMap.IoC;
using StructureMap;

namespace LiveActivityMap.App_Start
{
    public class StructureMapMvc
    {
        public static void Start()
        {
            StructureMapSetup.EnsureStructureMapIsReady();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(ObjectFactory.Container));

            // Comment this line in if we ever decide to use WebAPI
            //GlobalConfiguration.Configuration.DependencyResolver = new StructureMapDependencyResolver(ObjectFactory.Container);
        }
    }
}