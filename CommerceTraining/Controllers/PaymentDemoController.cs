using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Globalization;
using GiftCardPaymentProvider;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
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
            InitializeModel(viewModel);

            return View(viewModel);
        }

        public void InitializeModel(PaymentDemoViewModel viewModel)
        {
            ICart cart = _orderRepository.LoadOrCreateCart<ICart>(CustomerContext.Current.CurrentContactId, "Default");

            var shirtRef = _referenceConverter.GetContentLink("Long Sleeve Shirt White Small_1");
            viewModel.Shirt = _contentLoader.Get<ShirtVariation>(shirtRef);
            viewModel.ImageUrl = _assetUrlResolver.GetAssetUrl(viewModel.Shirt);
            viewModel.PayMethods = PaymentManager.GetPaymentMethodsByMarket(_currentMarket.GetCurrentMarket().MarketId.Value).PaymentMethod;
            viewModel.CartItems = cart.GetAllLineItems();
            viewModel.CartTotal = cart.GetTotal();

            viewModel.GiftCards = GiftCardService.GetClientGiftCards("TrainingGiftCard", (PrimaryKeyId)CustomerContext.Current.CurrentContactId);
        }

        public ActionResult UpdateCart(PaymentDemoViewModel viewModel)
        {
            InitializeModel(viewModel);
            var cart = _orderRepository.LoadOrCreateCart<ICart>(CustomerContext.Current.CurrentContactId, "Default");

            var lineItem = cart.GetAllLineItems().FirstOrDefault(x => x.Code == viewModel.Shirt.Code);

            if(lineItem == null)
            {
                lineItem = _orderGroupFactory.CreateLineItem(viewModel.Shirt.Code, cart);
                lineItem.Quantity = viewModel.PurchaseQuantity;
                lineItem.PlacedPrice = viewModel.Shirt.GetDefaultPrice().UnitPrice;
                cart.AddLineItem(lineItem);
            }
            else
            {
                var shipment = cart.GetFirstShipment();
                cart.UpdateLineItemQuantity(shipment, lineItem, viewModel.PurchaseQuantity);
            }
            
            _orderRepository.Save(cart);

            return RedirectToAction("Index");
        }

        public ActionResult SimulatePurchase(PaymentDemoViewModel viewModel)
        {
            var cart = _orderRepository.LoadOrCreateCart<ICart>(CustomerContext.Current.CurrentContactId, "Default");

            var primaryPayment = _orderGroupFactory.CreatePayment(cart);
            primaryPayment.PaymentMethodId = viewModel.SelectedPaymentId;
            primaryPayment.Amount = _orderGroupCalculator.GetTotal(cart).Amount;
            primaryPayment.PaymentMethodName = PaymentManager.GetPaymentMethod(viewModel.SelectedPaymentId).PaymentMethod[0].Name;

            if (viewModel.UseGiftCard)
            {
                var giftMethod = PaymentManager.GetPaymentMethodBySystemName("GiftCard", ContentLanguage.PreferredCulture.Name);
                var giftPayment = _orderGroupFactory.CreatePayment(cart);
                giftPayment.PaymentMethodId = giftMethod.PaymentMethod[0].PaymentMethodId;
                giftPayment.Amount = viewModel.GiftCardDebitAmt;
                giftPayment.ValidationCode = viewModel.RedemtionCode;
                giftPayment.PaymentMethodName = giftMethod.PaymentMethod[0].Name;
                
                PaymentProcessingResult giftPayResult = _paymentProcessor.ProcessPayment(cart, giftPayment, cart.GetFirstShipment());
                if (giftPayResult.IsSuccessful)
                {
                    primaryPayment.Amount -= giftPayment.Amount;
                    cart.AddPayment(giftPayment);
                }
                viewModel.GiftInfoMessage = giftPayResult.Message;
            }

            PaymentProcessingResult payResult = _paymentProcessor.ProcessPayment(cart, primaryPayment, cart.GetFirstShipment());

            if (payResult.IsSuccessful)
            {
                cart.AddPayment(primaryPayment);
                _orderRepository.SaveAsPurchaseOrder(cart);
                _orderRepository.Delete(cart.OrderLink);
            }

            InitializeModel(viewModel);
            viewModel.MessageOutput = payResult.Message;

            return View("Index", viewModel);
        }
    }
}