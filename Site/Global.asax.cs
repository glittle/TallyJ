﻿using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentSecurity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using TallyJ.Code;
using TallyJ.Code.UnityRelated;
using TallyJ.Controllers;
using Unity.Mvc3;

namespace TallyJ
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(UnityInstance.Container));
      
      UnityInstance.Container.RegisterControllers();

			DependencyResolver.SetResolver(new UnityDependencyResolver(UnityInstance.Container));
      
		  FixUpConnectionString();
			Bootstrapper.Initialise();

      SecurityConfigurator.Configure(configuration =>
      {
        // http://www.fluentsecurity.net/getting-started

        // Let Fluent Security know how to get the authentication status of the current user
        configuration.GetAuthenticationStatusFrom(() => HttpContext.Current.User.Identity.IsAuthenticated);

        configuration.ResolveServicesUsing(type => UnityInstance.Container.ResolveAll(type));

        // This is where you set up the policies you want Fluent Security to enforce on your controllers and actions

        configuration.ForAllControllers().DenyAnonymousAccess();

        configuration.For<PublicController>().Ignore();


        configuration.For<AfterController>().AddPolicy(new RequireElectionPolicy());
        
        configuration.For<BallotsController>().AddPolicy(new RequireElectionPolicy());
        configuration.For<BallotsController>().AddPolicy(new RequireLocationPolicy());

        configuration.For<BeforeController>().AddPolicy(new RequireElectionPolicy());
        
        configuration.For<DashboardController>().DenyAnonymousAccess();
        
        configuration.For<ElectionsController>().DenyAnonymousAccess();
        
        configuration.For<PeopleController>().AddPolicy(new RequireElectionPolicy());
        
        configuration.For<SetupController>().AddPolicy(new RequireElectionPolicy());
        
        configuration.For<AccountController>().DenyAuthenticatedAccess();
        configuration.For<AccountController>(x => x.LogOff()).DenyAnonymousAccess();
      });


			//AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

	  void FixUpConnectionString()
	  {
      var cnString = ConfigurationManager.ConnectionStrings["MainConnection"];

      var fi = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
      fi.SetValue(cnString, false);

      cnString.ConnectionString = cnString.ConnectionString + ";MultipleActiveResultSets=True";
	  }

	  public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
      filters.Add(new HandleSecurityAttribute(), 0);
      filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}", // URL with parameters
				new
					{
						controller = "Public",
						action = "Index",
						//id = UrlParameter.Optional
					} // Parameter defaults
				);
		}
	}
}