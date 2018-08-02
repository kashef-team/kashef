using Kashef.Services;
using Microsoft.Practices.Unity;

using System.Web.Http;
using System.Web.Mvc;

namespace Kashef.API.Models
{
    public static class UnityBootstrapper
    {
        private static IUnityContainer currentContainer = null;
        public static IUnityContainer Initialise()
        {
            currentContainer = BuildUnityContainer();

           // DependencyResolver.SetResolver(new UnityResolver(currentContainer));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(currentContainer);



            return currentContainer;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            IUnityContainer container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers 

            RegisterTypes(container);

            return container;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ICollectionService, CollectionManagmentService>();

            container.RegisterType<IRekognitionService, RekognitionService>();  
        }
        public static T ResolveInstance<T>()
        {
            return currentContainer.Resolve<T>();
        }

    }
}