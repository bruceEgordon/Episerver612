using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class PaymentDemoController : Controller
    {
        private ReferenceConverter _referenceConverter;
        private IContentLoader _contentLoader;
        private AssetUrlResolver _assetUrlResolver;
        private ICurrentMarket _currentMarket;

        public PaymentDemoController(ReferenceConverter referenceConverter, IContentLoader contentLoader,
            AssetUrlResolver assetUrlResolver, ICurrentMarket currentMarket)
        {
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
            _assetUrlResolver = assetUrlResolver;
            _currentMarket = currentMarket;
        }
        public ActionResult Index()
        {
            var viewModel = new PaymentDemoViewModel();
            ModelFiller(viewModel);

            return View(viewModel);
        }

        public void ModelFiller(PaymentDemoViewModel viewModel)
        {
            var shirtRef = _referenceConverter.GetContentLink("Long Sleeve Shirt White Small_1");
            viewModel.Shirt = _contentLoader.Get<ShirtVariation>(shirtRef);
            viewModel.ImageUrl = _assetUrlResolver.GetAssetUrl(viewModel.Shirt);
            viewModel.PayMethods = PaymentManager.GetPaymentMethodsByMarket(_currentMarket.GetCurrentMarket().MarketId.Value).PaymentMethod;
        }
    }
}