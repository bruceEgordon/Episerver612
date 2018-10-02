﻿using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class MarketsDemoController : Controller
    {
        private IMarketService _marketService;
        private ICurrentMarket _currentMarket;
        private ReferenceConverter _referenceConverter;
        private IContentLoader _contentLoader;
        private IOrderRepository _orderRepository;
        private IOrderGroupFactory _orderGroupFactory;
        private ITaxCalculator _taxCalculator;

        public MarketsDemoController(IMarketService marketService, ICurrentMarket currentMarket,
            ReferenceConverter referenceConverter, IContentLoader contentLoader,
            IOrderRepository orderRepository, IOrderGroupFactory orderGroupFactory,
            ITaxCalculator taxCalculator)
        {
            _marketService = marketService;
            _currentMarket = currentMarket;
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
            _orderRepository = orderRepository;
            _orderGroupFactory = orderGroupFactory;
            _taxCalculator = taxCalculator;
        }
        // GET: MarketsDemo
        public ActionResult Index()
        {
            var viewModel = new DemoMarketsViewModel();
            viewModel.MarketList = _marketService.GetAllMarkets();
            viewModel.SelectedMarket = _currentMarket.GetCurrentMarket();

            var shirtRef = _referenceConverter.GetContentLink("Long Sleeve Shirt White Small_1");
            viewModel.Shirt = _contentLoader.Get<ShirtVariation>(shirtRef);

            GetTaxInfo(viewModel);

            return View(viewModel);
        }

        public ActionResult ChangeDefaultMarket(string selectedMarket)
        {
            if (selectedMarket != null)
            {
                _currentMarket.SetCurrentMarket(new MarketId(selectedMarket));
            }
            var viewModel = new DemoMarketsViewModel();
            viewModel.MarketList = _marketService.GetAllMarkets();
            viewModel.SelectedMarket = _currentMarket.GetCurrentMarket();

            var shirtRef = _referenceConverter.GetContentLink("Long Sleeve Shirt White Small_1");
            viewModel.Shirt = _contentLoader.Get<ShirtVariation>(shirtRef);

            GetTaxInfo(viewModel);

            return View("Index", viewModel);
        }

        private void GetTaxInfo(DemoMarketsViewModel viewModel)
        {
            ICart cart = _orderRepository.LoadOrCreateCart<ICart>(CustomerContext.Current.CurrentContactId, "BogusCart");

            IOrderAddress bogusAddress = _orderGroupFactory.CreateOrderAddress(cart);
            //bogusAddress.CountryCode = "sv";
            bogusAddress.CountryCode = "usa";
            bogusAddress.City = "Stockholm";
            bogusAddress.CountryName = "Sweden";

            ILineItem lineItem = _orderGroupFactory.CreateLineItem(viewModel.Shirt.Code, cart);
            lineItem.Quantity = 1;
            lineItem.PlacedPrice = viewModel.Shirt.GetDefaultPrice().UnitPrice;
            lineItem.TaxCategoryId = viewModel.Shirt.TaxCategoryId;

            cart.AddLineItem(lineItem);

            viewModel.TaxAmount = _taxCalculator.GetSalesTax(lineItem, viewModel.SelectedMarket, bogusAddress, new Money(0m, viewModel.SelectedMarket.DefaultCurrency));
        }

        private decimal GetTaxOldSchool(ShirtVariation shirt, string languageCode, IOrderAddress orderAddress)
        {
            decimal decTaxTotal = 0;
            string taxCategory = CatalogTaxManager.GetTaxCategoryNameById((int)shirt.TaxCategoryId);
            IEnumerable<TaxValue> taxes = OrderContext.Current.GetTaxes(Guid.Empty, taxCategory, languageCode,  orderAddress);
            foreach (var tax in taxes)
            {
                decTaxTotal += (decimal)tax.Percentage * shirt.GetDefaultPrice().UnitPrice;
            }
            return decTaxTotal;
        }
    }
}