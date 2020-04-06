using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RobinOperasyonWebService
{
    public static class WebApiConfig
    {

       


        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //EnableCorsAttribute cors = new EnableCorsAttribute(origins: "*", headers: "Origin, X-Requested-With, Content-Type, Accept ", methods: "GET, POST");
            //config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           

        }

        
    }


}
