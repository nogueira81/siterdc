﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
//using rdc.Controllers;

namespace rdc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            /*
            routes.MapRoute( 
            "DefaultApi",
            "api/{controller}/{id}",
            new { controller = "Login", TCEP = "GetCep", id = RouteParameter.Optional }
            );*/
            //Inclído para que o rdc funcione com o IIS 5.1
            routes.Add(new Route
            (
                "{controller}/{action}/{id}",
                new RouteValueDictionary(new { action = "Index", id = (string)null }),
                new MvcRouteHandler()
            ));

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "RegistroLogin", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Login", action = "Registro", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "RegRecFor", // Route name
            "{controller}/{action}/{id}", // URL with parameters
            new { controller = "Reclamar/", action = "listaFornec", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "RegistroCEPLogin", // Route name
            "{controller}/{action}/{id}", // URL with parameters
            new { controller = "Login", action = "GetCep", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "RegAreaCliCEPInt",
            "{controller}/AreaCliEdit/{action}/{id}",
            new { controller = "Clientes", action = "GetCep", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            "RegistroCEPInt",
            "{controller}/Edit/{action}/{id}",
            new { controller = "Clientes", action = "GetCep", id = UrlParameter.Optional }
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}