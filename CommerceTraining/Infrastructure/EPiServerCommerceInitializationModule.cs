using System.Web.Routing;

using EPiServer.Commerce.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using CommerceTraining.Infrastructure;
using System.Web.Mvc;
using Mediachase.Commerce.Engine.Events;
using CommerceTraining.Infrastructure.Pricing;
using Mediachase.Commerce.Pricing;
using Mediachase.Commerce;
using EPiServer;
using EPiServer.Core;
using CommerceTraining.Models.Pages;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Catalog;
using EPiServer.DataAbstraction;
using Mediachase.Commerce.Orders;
using EPiServer.Commerce.Order;
using EPiServer.Find.ClientConventions;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing.Internal;
using CommerceTraining.Infrastructure.Promotions;
using System;
using EPiServer.Commerce.Marketing;
using System.Linq;
using EPiServer.Globalization;
using EPiServer.Core.Internal;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Customers;
using System.Collections.Generic;
using CommerceTraining.Infrastructure.CartAndCheckout;
using Mediachase.Commerce.Catalog.Events;
using EPiServer.Events.Clients;
using EPiServer.Events;

namespace CommerceTraining.Infrastructure
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class EPiServerCommerceInitializationModule : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // routing fund.
            //CatalogRouteHelper.MapDefaultHierarchialRouter(RouteTable.Routes,, false);

            // routing adv.
            CatalogRouteHelper.MapDefaultHierarchialRouter(RouteTable.Routes, () =>
            {
                // the scheduled job still have troubles
                var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

                // If epi-Find-error in exercise, it's the "catalog start"
                var startPage = new ContentReference(9);

                //var startPage = contentLoader.Get<PageData>(ContentReference.StartPage);
                if (startPage == null) // should maybe check for the "setting-prop"
                {
                    return ContentReference.WasteBasket; //.StartPage;
                }

                //var homePage = startPage as StartPage;

                //return homePage != null ? homePage.Settings.catalogStartPageLink : ContentReference.StartPage;

                // if Find issues
                return startPage;
            }
            , false);

            #region Partial Routing Simplified... Conceptually
            //CatalogRouteHelper.MapDefaultHierarchialRouter(RouteTable.Routes, () =>
            //{
            //    var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            //    var startPage = contentLoader.Get<StartPage>(ContentReference.StartPage);
            //    return startPage.Settings.catalogStartPageLink;
            //}
            //, false);
            #endregion

            #region Events CMS & ECF

            // Nice for Entry Pricing and Inventory updates
            CatalogKeyEventBroadcaster e =
                ServiceLocator.Current.GetInstance<CatalogKeyEventBroadcaster>();

            e.PriceUpdated += e_PriceUpdated;
            e.InventoryUpdated += e_InventoryUpdated;
            
            Event.Get(CatalogEventBroadcaster.CommerceProductUpdated).Raised += EB;
            

            // this is the old one for Orders
            //OrderContext.Current.OrderGroupUpdated += Current_OrderGroupUpdated; 

            // Good stuff for Orders ...\Infrastructure\CartAndCheckout\NewOrderEvents.cs
            //IOrderRepositoryCallback orc = 
            //    ServiceLocator.Current.GetInstance<IOrderRepositoryCallback>();

            IContentEvents e2 = ServiceLocator.Current.GetInstance<IContentEvents>(); // ...the way to go
            e2.RequestedApproval += E2_RequestedApproval;
            e2.PublishedContent += E2_PublishedContent;

            #endregion

            // new in 10.1.0
            SetPromotionExclusions(context);
        }

        #region Event handlers


        private void EB(object sender, EventNotificationEventArgs e)
        {
            EventReciever.RecordEntryChange(sender, e);
        }

        private void E2_PublishedContent(object sender, ContentEventArgs e)
        {
            if (e.Content.GetOriginalType().Name == "ShirtNode")
            {
                EventReciever.RecordCatalogItemApprovalRequest(sender, e);
            }
        }

        private void E2_RequestedApproval(object sender, ContentEventArgs e)
        {

        }

        void Current_OrderGroupUpdated(object sender, OrderGroupEventArgs e)
        {
            EventReciever.RecordOrderGroupEvent(sender, e);
        }

        void e_InventoryUpdated(object sender, InventoryUpdateEventArgs e)
        {
            EventReciever.RecordInventoryEvent(sender, e);
        }

        //void Instance_PublishingContent(object sender, ContentEventArgs e)
        //{

        //}

        void e_PriceUpdated(object sender, PriceUpdateEventArgs e)
        {
            EventReciever.RecordPriceEvent(sender, e);
        }

        #endregion

        #region Promotions

        private void SetPromotionExclusions(InitializationEngine context)
        {
            /*
            // works in version 10.1.0 +
            var filter = context.Locate.Advanced.GetInstance<EntryFilterSettings>();
            var f2 = ServiceLocator.Current.GetInstance<EntryFilterSettings>();

            // never allow this type to have promotions
            filter.AddFilter<ClothesAccessory>(x => false);

            // specific items
            var codes = new string[] { "TheSkuWithSize_1", "Trousers-with-buttons_1" };
            filter.AddFilter<EntryContentBase>(x => !codes.Contains(x.Code));
            */
        }

        private void DisablePromotionTypes(InitializationEngine context)
        {
            var promotionTypeHandler = 
                context.Locate.Advanced.GetInstance<PromotionTypeHandler>();

            /*
            //To disable all built-in promotion types
            promotionTypeHandler.DisableBuiltinPromotions();

            // have a look later
            IEnumerable<Type> types = promotionTypeHandler.GetAllPromotionTypes();
            var T = types.Where(tt => tt.BaseType == typeof(EntryPromotion));
            promotionTypeHandler.DisablePromotions(T);
            */

        }
        
        #endregion

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.StructureMap()));

            // newer
            //context.Services.AddTransient<ICurrentMarket, MyCustomCurrentMarket>();

            context.Services.AddSingleton<IPriceOptimizer, CustomPriceOptimizer>();

            context.Services.AddSingleton<ITaxCalculator, CustomTaxCalculator>();

            context.Services.AddSingleton<IPriceService, MyPriceService>();

            context.Services.AddSingleton<PromotionEngineContentLoader, CustomPromotionEngineContentLoader>();
            
            //context.Services.RemoveAll<ThumbnailManager>(); // "Internal" ... had a question about it

            // older, gone in 12 ... but still works in 11 
            //context.Container.Configure(c =>
            //{
            //    //c.For<ICurrentMarket>().Singleton().Use<MyCustomCurrentMarket>();
            //    c.For<IPriceService>().Singleton().Use<MyPriceService>();

            //    // If we need to override (prob. does)
            //    c.For<PromotionEngineContentLoader>().Singleton()
            //    .Use<CustomPromotionEngineContentLoader>();

            //    #region QS

            //    //c.For<Func<string, CartHelper>>()
            //    //.HybridHttpOrThreadLocalScoped()
            //    //.Use(() => new Func<string, CartHelper>((cartName) => new CartHelper(cartName, PrincipalInfo.CurrentPrincipal.GetContactId())));

            //    ////Register for auto injection of edit mode check, should be default life cycle (per request)
            //    //c.For<Func<bool>>()
            //    //.Use(() => new Func<bool>(() => PageEditing.PageIsInEditMode));

            //    //c.For<IUpdateCurrentLanguage>()
            //    //    .Singleton()
            //    //    .Use<LanguageService>()
            //    //    .Setter<IUpdateCurrentLanguage>()
            //    //    .Is(x => x.GetInstance<UpdateCurrentLanguage>());

            //    //c.For<Func<CultureInfo>>().Use(() => new Func<CultureInfo>(() => ContentLanguage.PreferredCulture));

            //    //Func<IOwinContext> owinContextFunc = () => HttpContext.Current.GetOwinContext();
            //    //c.For<ApplicationUserManager>().Use(() => owinContextFunc().GetUserManager<ApplicationUserManager>());
            //    //c.For<ApplicationSignInManager>().Use(() => owinContextFunc().Get<ApplicationSignInManager>());
            //    //c.For<IAuthenticationManager>().Use(() => owinContextFunc().Authentication);
            //    //c.For<IOwinContext>().Use(() => owinContextFunc());
            //    //c.For<IModelBinderProvider>().Use<ModelBinderProvider>();

            //    #endregion

            //    // latest addition
            //    /*context.Services.Intercept<IUpdateCurrentLanguage>(
            //        (locator, defaultImplementation) =>
            //            new LanguageService(
            //                locator.GetInstance<ICurrentMarket>(),
            //                locator.GetInstance<MyCookieService>(),
            //                defaultImplementation,
            //                locator.GetInstance<RequestContext>()));*/
            //});

            
        }

        public void Register(TemplateModelCollection viewTemplateModelRegistrator)
        {
            // ECF?
            //viewTemplateModelRegistrator.Add(typeof(AccessoryNode), new TemplateModel
            //{
            //    Name = "SpecialAccessoryRendering",
            //    //Tags = new[] { Global.ContentAreaTags.TwoThirdsWidth, Global.ContentAreaTags.FullWidth },
            //    AvailableWithoutTag = true,
            //    Path = "~/Views/ViewsNoController/StrangeAccessoriesView.cshtml"

            //});
        }
    }
}
