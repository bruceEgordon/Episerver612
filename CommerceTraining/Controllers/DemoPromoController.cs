using CommerceTraining.Models.ViewModels;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Order.Internal;
using EPiServer.Core;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class DemoPromoController : Controller
    {
        private IPromotionEngine _promoEngine;
        private ReferenceConverter _refConverter;
        private ICurrentMarket _currentMarket;

        public DemoPromoController(IPromotionEngine promotionEngine, ReferenceConverter referenceConverter, ICurrentMarket currentMarket)
        {
            _promoEngine = promotionEngine;
            _refConverter = referenceConverter;
            _currentMarket = currentMarket;
        }
        public ActionResult Index()
        {
            var viewModel = new DemoPromoViewModel();
            var market = _currentMarket.GetCurrentMarket();
            //var items = new List<ContentReference>();
            //var shirtRef1 = _refConverter.GetContentLink("Long Sleeve Shirt White Small_1");
            //var shirtRef2 = _refConverter.GetContentLink("Long-Sleeve-Shirt-Blue-Medium_1");
            //items.Add(shirtRef1);
            //items.Add(shirtRef2);
            //viewModel.Rewards = _promoEngine.Evaluate(items, market, market.DefaultCurrency, RequestFulfillmentStatus.All);

            var inMemOrderGroup = new InMemoryOrderGroup(market, market.DefaultCurrency);
            var inMemLineItem1 = new InMemoryLineItem();
            inMemLineItem1.Code = "Long Sleeve Shirt White Small_1";
            inMemLineItem1.Quantity = 2;
            //var inMemLineItem2 = new InMemoryLineItem();
            //inMemLineItem2.Code = "Long-Sleeve-Shirt-Blue-Medium_1";
            //inMemLineItem2.Quantity = 1;
            inMemOrderGroup.GetFirstShipment().LineItems.Add(inMemLineItem1);
            //inMemOrderGroup.GetFirstShipment().LineItems.Add(inMemLineItem2);
            var promoSettings = new PromotionEngineSettings(RequestFulfillmentStatus.All, true);
            viewModel.Rewards = _promoEngine.Run(inMemOrderGroup, promoSettings);
            viewModel.InMemCart = inMemOrderGroup;
            return View(viewModel);
        }
    }
}