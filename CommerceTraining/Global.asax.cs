﻿using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace CommerceTraining
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }

        protected override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.MapRoute("SearchDemoRoute", "searchdemo/{action}", new { controller = "SearchDemo", action = "Index" });
            routes.MapRoute("FindDemoRoute", "findsearchdemo/{action}", new { controller = "FindSearchDemo", action = "Index" });
            routes.MapRoute("MarketsDemoRoute", "marketsdemo/{action}", new { controller = "MarketsDemo", action = "Index" });
            routes.MapRoute("ClassDemos", "classDemos/{controller}/{action}", new { action = "Index" });
        }
    }
}