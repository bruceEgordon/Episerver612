using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Customers;
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
        private IPaymentProcessor _paymentProcessor;
        private IOrderRepository _orderRepository;
        private IOrderGroupFactory _orderGroupFactory;
        private IOrderGroupCalculator _orderGroupCalculator;

        public PaymentDemoController(ReferenceConverter referenceConverter, IContentLoader contentLoader,
            AssetUrlResolver assetUrlResolver, ICurrentMarket currentMarket, IPaymentProcessor paymentProcessor,
            IOrderRepository orderRepository, IOrderGroupFactory orderGroupFactory, IOrderGroupCalculator orderGroupCalculator)
        {
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
            _assetUrlResolver = assetUrlResolver;
            _currentMarket = currentMarket;
            _paymentProcessor = paymentProcessor;
            _orderRepository = orderRepository;
            _orderGroupFactory = orderGroupFactory;
            _orderGroupCalculator = orderGroupCalculator;
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

        public ActionResult SimulatePurchase(PaymentDemoViewModel viewModel)
        {
            ModelFiller(viewModel);

            ICart cart = _orderRepository.LoadOrCreateCart<ICart>(CustomerContext.Current.CurrentContactId, "BogusCart");

            ILineItem lineItem = _orderGroupFactory.CreateLineItem(viewModel.Shirt.Code, cart);
            lineItem.Quantity = 1;
            lineItem.PlacedPrice = viewModel.Shirt.GetDefaultPrice().UnitPrice;

            cart.AddLineItem(lineItem);

            //var selectedPayment = PaymentManager.GetPaymentMethod(viewModel.SelectedPaymentId).PaymentMethod.First();
            var payment = _orderGroupFactory.CreatePayment(cart);
            payment.PaymentMethodId = viewModel.SelectedPaymentId;
            payment.Amount = _orderGroupCalculator.GetTotal(cart).Amount;
            cart.AddPayment(payment);

            PaymentProcessingResult payResult = _paymentProcessor.ProcessPayment(cart, cart.GetFirstForm().Payments.First(), cart.GetFirstShipment());

            viewModel.MessageOutput = payResult.Message;

            return View("Index", viewModel);
        }
    }
}