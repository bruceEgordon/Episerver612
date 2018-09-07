using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Pages;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce;
using Mediachase.Commerce.Website.Helpers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Engine;
using System;
using EPiServer.Security;
using Mediachase.Commerce.Customers;
using EPiServer.ServiceLocation;
using EPiServer.Globalization;
using System.Globalization;
using Mediachase.Commerce.Core;
using CommerceTraining.SupportingClasses;
using EPiServer.Find;
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.BusinessFoundation.Data;
using System.Messaging;
using Mediachase.Commerce.Orders.Exceptions;
using CommerceTraining.Infrastructure.CartAndCheckout;
using EPiServer.Commerce.Marketing;
using CommerceTraining.Models.Promotions;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Markets;
//using CommerceTraining.Infrastructure.Shipping;
//using Mediachase.FileUploader.Configuration;

namespace CommerceTraining.Controllers
{
    public class CheckOutController : OrderControllerBase<CheckOutPage>
    {
        #region Properties added for Adv.

        // added for Adv.
        private string AgreeSplitShip { get; set; } // not used right now
        private static bool AddSecondShipment { get; set; }
        private static bool IsOnLine { get; set; }

        // for test both to true if logged in
        //bool useGiftCard = false; // toggle to true for test
        //bool addSecondPayment = false; // initially...

        protected string frontEndMessage = String.Empty; // messages from BF-GiftCard
        protected bool chkGiftCard = false;
        protected bool poToQueue = true; // toggle for now...

        #endregion

        // kind of helper/clean up here
        CartAndCheckoutService ccService = new CartAndCheckoutService(); 

        public CheckOutController(
              IOrderRepository orderRepository
            , IOrderGroupFactory orderGroupFactory // RoCe: change
            , IOrderGroupCalculator orderGroupCalculator
            , IContentLoader contentLoader
            , ILineItemCalculator lineItemCalculator
            , IPlacedPriceProcessor placedPriceProcessor
            , IInventoryProcessor inventoryProcessor
            , ILineItemValidator lineItemValidator
            , IPromotionEngine promotionEngine
            , ICurrentMarket currentMarket
            , IPaymentProcessor paymentProcessor)
            : base(
                orderRepository, orderGroupFactory, orderGroupCalculator, contentLoader
                , lineItemCalculator, placedPriceProcessor, inventoryProcessor
                , lineItemValidator, promotionEngine, currentMarket, paymentProcessor)
        {
            //_currentMarket = currentMarket;
            //_contentLoader = contentLoader;
        }

        public ActionResult Index(CheckOutPage currentPage, bool passedAlong) // add the bool for accepting split Ship (Letter - Trousers) 
        {
            IsOnLine = CheckIfOnLine.IsInternetAvailable; // for Find ... if on the bus
            //IsOnLine = true;
            // Load the cart, it should be one there
            ICart cart = base.GetCart();
            if (cart == null)
            {
                throw new InvalidOperationException("No cart found"); // make nicer
            }

            // Should perhaps validate the cart & clean up... or not
            //ccService.CleanUpCart(cart);

            var model = new CheckOutViewModel(currentPage)
            {
                // ToDo: get shipments & payments
                PaymentMethods = GetPaymentMethods(),
                ShipmentMethods = GetShipmentMethods(),
                ShippingRates = GetShippingRates(),
                ShippingMethodInfo = CheckIfShippingMethodIsOkay() // not much done in this one, yet
            };

            return View(model);
        }

        #region From Fund

        private IEnumerable<ShippingRate> GetShippingRates()
        {
            List<ShippingRate> shippingRates = new List<ShippingRate>();
            IEnumerable<ShippingMethodDto.ShippingMethodRow> shippingMethods = GetShipmentMethods();

            foreach (var item in shippingMethods)
            {
                shippingRates.Add(new ShippingRate(item.ShippingMethodId, item.DisplayName
                    //, new Money(item.BasePrice, item.Currency).Round(2)) // obsoleted
                    // new style below
                    , new Money(item.BasePrice, _currentMarket.GetCurrentMarket()
                    .DefaultCurrency)
                    .Round()));
            }

            return shippingRates;
        }

        private IEnumerable<PaymentMethodDto.PaymentMethodRow> GetPaymentMethods()
        {
            string lang = ContentLanguage.PreferredCulture.TwoLetterISOLanguageName;
            return new List<PaymentMethodDto.PaymentMethodRow>(
                  PaymentManager
                  .GetPaymentMethodsByMarket(_currentMarket.GetCurrentMarket().MarketId.Value)
                  .PaymentMethod);
        }

        private IEnumerable<ShippingMethodDto.ShippingMethodRow> GetShipmentMethods()
        {
            IMarket market = _currentMarket.GetCurrentMarket();
            //CultureInfo cult = market.DefaultLanguage;
            //string str = cult.TwoLetterISOLanguageName;
            var str = market.DefaultLanguage.TwoLetterISOLanguageName;
            //string str = _currentMarket.GetCurrentMarket().DefaultLanguage.TwoLetterISOLanguageName;

            //string lang = ContentLanguage.PreferredCulture.TwoLetterISOLanguageName; // ...or however we like it
            return new List<ShippingMethodDto.ShippingMethodRow>(
                ShippingManager
                .GetShippingMethodsByMarket(market.MarketId.Value, false)
                //.ShippingMethod.Where(s => s.LanguageId == str)); // optionally
                .ShippingMethod);
        }

        #endregion

        private string CheckIfShippingMethodIsOkay()
        {
            // ...could use this one for ship-info, jibberish right now
            // would like a check ShippingOptionParameters and other stuff here
            return "Check on shipments... upcoming"; // ...for now
        }

        // This method is about what we ended up with in "Fund." - with a few changes done for Adv.
        public ActionResult CheckOut(CheckOutViewModel model)
        {
            // SplitPay is in a Session-variable (bool)
            string paymentProcessResult = String.Empty;

            // Load the cart, it should be one there
            var cart = _orderRepository.Load<ICart>(GetContactId(), "Default").FirstOrDefault();
            if (cart == null)
            {
                throw new InvalidOperationException("No cart found"); // make nicer
            }

            #region What actually happens when loading a cart - Adv.

            //Cart cart = OrderContext.Current.GetCart(
            //    "SomeCart"
            //    , CustomerContext.Current.CurrentContactId
            //    , MarketId.Default); // ...is still what happens in behind

            #endregion

            // should clean-up among payments here if the  first time failed - Qty 10 test
            // quick fixdon in the base class

            // From Fund
            IOrderAddress theAddress = AddAddressToOrder(cart);

            // ToDo: Added this field for Adv. & Find ... doing it simple now using one Address 
            // The address is for Find, but we need to add it to MDP to be able to use it properly
            // This is a Serialized cart, so doesn't crash if the field is not added to MDP 
            theAddress.Properties["AddressType"] = "Shipping";

            #region Ship & Pay from Fund

            // ToDo: Define Shipping - From Fund
            AdjustFirstShipmentInOrder(cart, theAddress, model.SelectedShipId); // ...as a Shipment is added by epi 

            // ToDo: Define Payment - From Fund
            AddPaymentToOrder(cart, model.SelectedPayId); // ...as this is not added by default

            #endregion

            #region Split Pay

            // RoCe: Fix this - addSecondPayment comes in as a param (bool) 
            // ... force for now if BF-Card is found ... using Session
            if ((bool)Session["SecondPayment"] == true)
            {
                ccService.AddSecondPaymentToOrder(cart);
            }

            // gathered info
            this.frontEndMessage = ccService.FrontEndMessage;

            #endregion

            // Possible change of the cart... adding this 
            // would have this done if a flag were set
            var cartReference = _orderRepository.Save(cart);

            // Original Fund... (with additions)
            IPurchaseOrder purchaseOrder;
            OrderReference orderReference;

            #region Transaction Scope

            using (var scope = new Mediachase.Data.Provider.TransactionScope()) // one in BF, also
            {
                var validationIssues = new Dictionary<ILineItem, ValidationIssue>();

                // Added - sets a lock on inventory... 
                // ...could come earlier (outside tran) depending on TypeOf-"store"
                _inventoryProcessor.AdjustInventoryOrRemoveLineItem(cart.GetFirstShipment()
                    , OrderStatus.InProgress, (item, issue) => validationIssues.Add(item, issue));

                if (validationIssues.Count >= 1)
                {
                    throw new Exception("Not possible right now"); // ...change approach
                }

                // just checking the cart in watch window
                var theShipping = cart.GetFirstShipment();
                var theLineItems = cart.GetAllLineItems();
                var firstPayment = cart.GetFirstForm().Payments.First(); // no "GetFirstPayment()"
                var theforms = cart.Forms;

                //_lineItemCalculator.GetDiscountedPrice()
                // second payment is added in the Trousers-Controller
                // ...fiddling with the GiftCarde as well

                // before 11
                //cart.ProcessPayments(_paymentProcessor, _orderGroupCalculator);

                // Gets the older one
                //IEnumerable<PaymentProcessingResult> theResult 
                //    = cart.ProcessPayments(_paymentProcessor, _orderGroupCalculator);
                //paymentProcessResult = theResult.First().Message;

                PaymentProcessingResult otherResult =
                _paymentProcessor.ProcessPayment(cart, cart.GetFirstForm().Payments.First(), cart.GetFirstShipment());

                frontEndMessage += otherResult.Message;

                if (otherResult.IsSuccessful)
                {
                    IPayment thePay = cart.GetFirstForm().Payments.First();
                    thePay.Status = PaymentStatus.Processed.ToString();
                }
                else
                {
                    IPayment thePay = cart.GetFirstForm().Payments.First();
                    thePay.Status = PaymentStatus.Failed.ToString();
                    throw new System.Exception("Bad payment"); // could have more grace
                }
                // A custom "shipping-processor" created (needs to do OldSchool-things right now)
                // Have a custom (very simple) Shipping-Provider added to the solution.
                // the processor can be cleaned up a lot, no need to show it

                // Custom thing... Error in 11... on currency... check later
                //ShippingProcessor p = new ShippingProcessor();
                //p.ProcessShipments(cart as OrderGroup); // have to go Old-school

                // ...only one form, still
                var totalProcessedAmount = cart.GetFirstForm().Payments.Where
                    (x => x.Status.Equals(PaymentStatus.Processed.ToString())).Sum(x => x.Amount);

                // nice extension method
                var cartTotal = cart.GetTotal();

                // Do inventory - decrement or put back in stock
                if (totalProcessedAmount != cart.GetTotal(_orderGroupCalculator).Amount)
                {
                    // put back the reserved request
                    _inventoryProcessor.AdjustInventoryOrRemoveLineItem(cart.GetFirstShipment()
                        , OrderStatus.Cancelled, (item, issue) => validationIssues.Add(item, issue));

                    #region OldSchool Inventory - no demo,just checking ... were undocumented and wrong in SDK
                    //List<InventoryRequestItem> requestItems = new List<InventoryRequestItem>(); // holds the "items"
                    //InventoryRequestItem requestItem = new InventoryRequestItem();

                    //// calls for some logic
                    //requestItem.RequestType = InventoryRequestType.Cancel; // as a demo
                    //requestItem.OperationKey = reqKey;

                    //requestItems.Add(requestItem);

                    //InventoryRequest inventoryRequest = new InventoryRequest(DateTime.UtcNow, requestItems, null);
                    //InventoryResponse inventoryResponse = _invService.Service.Request(inventoryRequest);

                    //InventoryRecord rec4 = _invService.Service.Get(LI.Code, wh.Code);
                    #endregion OldSchool

                    throw new InvalidOperationException("Wrong amount"); // maybe change approach
                }

                // RoCe: have to do Promos here also ... move stuff from cart to "base"

                // simulation... should be an "else"
                cart.GetFirstShipment().OrderShipmentStatus = OrderShipmentStatus.InventoryAssigned;
                // decrement inventory and let it go
                _inventoryProcessor.AdjustInventoryOrRemoveLineItem(cart.GetFirstShipment()
                    , OrderStatus.Completed, (item, issue) => validationIssues.Add(item, issue));

                // Should do the ClubCard thing here - ClubMembers are logged in
                // PaymentMethodName = "GiftCard"
                if (CustomerContext.Current.CurrentContact != null)
                {
                    // check if GiftCard was used, don't give bonus for that payment
                    IEnumerable<IPayment> giftCardPayment = cart.GetFirstForm().Payments.Where
                        (x => x.PaymentMethodName.Equals("GiftCard"));

                    if (giftCardPayment.Count() >= 1)
                    {
                        ccService.UpdateClubCard(cart, totalProcessedAmount - giftCardPayment.First().Amount);
                    }
                    else
                    {
                        // no GiftCard, but collecting points
                        ccService.UpdateClubCard(cart, totalProcessedAmount);
                    }
                }

                #region OldSchool Inventory check

                //List<InventoryRequestItem> requestItems1 = new List<InventoryRequestItem>(); // holds the "items"
                //InventoryRequestItem requestItem1 = new InventoryRequestItem();

                //// calls for some logic
                //requestItem1.RequestType = InventoryRequestType.Complete; // as a demo
                //requestItem1.OperationKey = reqKey;

                //requestItems1.Add(requestItem1);

                //InventoryRequest inventoryRequest1 = new InventoryRequest(DateTime.UtcNow, requestItems1, null);
                //InventoryResponse inventoryResponse1 = _invService.Service.Request(inventoryRequest1);

                //InventoryRecord rec3 = _invService.Service.Get(LI.Code, wh.Code); // inventory reserved, but not decremented

                #endregion OldSchool

                orderReference = _orderRepository.SaveAsPurchaseOrder(cart);
                _orderRepository.Delete(cart.OrderLink);

                //InventoryRecord rec5 = _invService.Service.Get(LI.Code, wh.Code); // just checking

                scope.Complete();
            } // End Tran

            #endregion

            #region JustChecking

            //Guid custLock;
            //OrderGroupLockManager.IsOrderGroupLocked(orderReference.OrderGroupId, out (Guid)CustomerContext.Current.CurrentContact.PrimaryKeyId));

            /*
            OrderGroupLockManager.LockOrderGroup(orderReference.OrderGroupId
                , (Guid)CustomerContext.Current.CurrentContact.PrimaryKeyId);

            OrderGroupLockManager.UnlockOrderGroup(orderReference.OrderGroupId);
            */
            #endregion

            // just demoing (Find using this further down)
            purchaseOrder = _orderRepository.Load<IPurchaseOrder>(orderReference.OrderGroupId);

            // check the below
            var theType = purchaseOrder.OrderLink.OrderType;
            var toString = purchaseOrder.OrderLink.ToString(); // Gets ID and Type ... combined

            #region ThisAndThat - from Fund

            // should do some with OrderStatusManager

            OrderStatus poStatus;
            poStatus = purchaseOrder.OrderStatus;
            //purchaseOrder.OrderStatus = OrderStatus.InProgress;

            //var info = OrderStatusManager.GetPurchaseOrderStatus(PO);

            var shipment = purchaseOrder.GetFirstShipment();
            var status = shipment.OrderShipmentStatus;

            //shipment. ... no that much to do
            shipment.OrderShipmentStatus = OrderShipmentStatus.InventoryAssigned;

            #region Old-School, but some useful stuff

            //OrderStatusManager.ReleaseOrderShipment(purchaseOrder.GetFirstShipment() as Shipment);
            //OrderStatusManager.ReleaseOrderShipment(PO.OrderForms[0].Shipments[0]); // it gets released
            //OrderStatusManager.HoldOrder(PO); // it gets hold
            //OrderStatusManager.

            // seems to be a DTO involved... don't neeed to set the time like this... could use the new ordernote
            //OrderNotesManager.AddNoteToPurchaseOrder(PO, DateTime.UtcNow.ToShortDateString() + " done some for shipping", OrderNoteTypes.System, CustomerContext.Current.CurrentContactId);

            //            _orderRepository.Save(purchaseOrder); // check if it's like before ... yes it is needed to save again

            #endregion

            var notes = purchaseOrder.Notes; // IOrderNote is 0
            // RoCe - possible BUG
            // PO.OrderNotes works and contain the note above
            //IOrderNote theNewNote = 
            Mediachase.Commerce.Orders.OrderNote otherNote = new OrderNote //IOrderNote 
            {
                // Created = DateTime.Now, // do we need to set this ?? Nope .ctor does
                CustomerId = new Guid(), // can set this - regarded
                Detail = "Order ToString(): " + toString + " - Shipment tracking number: " + shipment.ShipmentTrackingNumber,
                LineItemId = purchaseOrder.GetAllLineItems().First().LineItemId,
                // OrderGroupId = 12, R/O - error
                // OrderNoteId = 12, // can define it, but it's disregarded - no error
                Title = "Some title",
                Type = OrderNoteTypes.Custom.ToString()
            }; // bug issued

            purchaseOrder.Notes.Add(otherNote); // void back
            purchaseOrder.ExpirationDate = DateTime.Now.AddMonths(1);


            PurchaseOrder oldPO = (PurchaseOrder)purchaseOrder;
            //oldPO.OrderAddresses.

            // yes, still need to come after adding notes
            _orderRepository.Save(purchaseOrder); // checking down here ... yes it needs to be saved again

            #endregion

            string conLang0 = ContentLanguage.PreferredCulture.Name;
            //string conLang1 = ContentLanguage.PreferredCulture.NativeName;
            //string conLang2 = ContentLanguage.PreferredCulture.TwoLetterISOLanguageName;

            // original shipment, could rewrite and get the dto so it can be used for the second shipment also
            // or grab the dto when loading into the dropdowns
            ShippingMethodDto.ShippingMethodRow theShip =
                ShippingManager.GetShippingMethod(model.SelectedShipId).ShippingMethod.First();

            #region Find & Queue plumbing

            // would be done async...
            if (IsOnLine) // just checking if the below is possible, if we have network access
            {
                // index PO and addresses for BoughtThisBoughtThat & demographic analysis
                IClient client = Client.CreateFromConfig(); // native
                FindQueries Qs = new FindQueries(client, true);
                Qs.OrderForFind(purchaseOrder);
            }

            if (poToQueue) // could have better tran-integrity, Extraction later in PO_Extract.sln/Sheduled job
            {
                // ToDo: Put a small portion of data from the PO to msmq, will eventually (out-of-process) go to the ERP
                string QueueName = ".\\Private$\\MyQueue";
                MessageQueue Q1 = new MessageQueue(QueueName);
                MyMessage m = new MyMessage()
                {
                    poNr = purchaseOrder.OrderNumber,
                    status = purchaseOrder.OrderStatus.ToString(),
                    orderGroupId = orderReference.OrderGroupId
                };

                Q1.Send(m);
            }

            #endregion

            // Final steps, navigate to the order confirmation page
            StartPage home = _contentLoader.Get<StartPage>(ContentReference.StartPage);
            ContentReference orderPageReference = home.Settings.orderPage;

            string passingValue = frontEndMessage + paymentProcessResult + " - " + purchaseOrder.OrderNumber;
            return RedirectToAction("Index", new { node = orderPageReference, passedAlong = passingValue });
        }

        //private void AddClubPoints()
        //{
        //    if (CustomerContext.Current.CurrentContact != null)
        //    {
        //        //this.contact = CustomerContext.Current.CurrentContact;
        //        //this.AddClubCardPoints(OrderGroup);

        //    }
        //}

        #region From Fund. Address & Shipping

        private IOrderAddress AddAddressToOrder(ICart cart)
        {
            IOrderAddress shippingAddress;

            if (CustomerContext.Current.CurrentContact == null)
            {
                // Anonymous... one way of "doing it"... for example, if no other address exist
                var shipment = cart.GetFirstShipment(); // ... moved to shipment - prev. = .OrderAddresses.Add(

                if (shipment.ShippingAddress != null)
                {
                    //return false/true; // Should clean up? ... did earlier for ship & pay
                }


                //Shipment oldShip = shipment as Shipment;
                shippingAddress = shipment.ShippingAddress = // should be an else here... below?
                    new OrderAddress
                    {
                        CountryCode = "USA",
                        CountryName = "United States",
                        Name = "SomeCustomerAddressName",
                        DaytimePhoneNumber = "123456",
                        FirstName = "John",
                        LastName = "Smith",
                        Email = "John@company.com",
                    };

            }
            else
            {
                // Logged in
                if (CustomerContext.Current.CurrentContact.PreferredShippingAddress == null)
                {
                    // no pref. address set... so we set one for the contact
                    CustomerAddress newCustAddress =
                        CustomerAddress.CreateForApplication(Mediachase.Commerce.Core.AppContext.Current.ApplicationId);
                    newCustAddress.AddressType = CustomerAddressTypeEnum.Shipping; // mandatory
                    newCustAddress.ContactId = CustomerContext.Current.CurrentContact.PrimaryKeyId;
                    newCustAddress.CountryCode = "SWE";
                    newCustAddress.CountryName = "Sweden";
                    newCustAddress.Name = "new customer address"; // mandatory
                    newCustAddress.DaytimePhoneNumber = "123456";
                    newCustAddress.FirstName = CustomerContext.Current.CurrentContact.FirstName;
                    newCustAddress.LastName = CustomerContext.Current.CurrentContact.LastName;
                    newCustAddress.Email = "GuitarWorld@Thule.com";

                    // note: Line1 & City is what is shown in CM at a few places... not the Name
                    CustomerContext.Current.CurrentContact.AddContactAddress(newCustAddress);
                    CustomerContext.Current.CurrentContact.SaveChanges();

                    // ... needs to be in this order
                    CustomerContext.Current.CurrentContact.PreferredShippingAddress = newCustAddress;
                    CustomerContext.Current.CurrentContact.SaveChanges(); // need this ...again 

                    // then, for the cart
                    //.Cart.OrderAddresses.Add(new OrderAddress(newCustAddress)); - OLD
                    shippingAddress = new OrderAddress(newCustAddress); // - NEW
                }
                else
                {
                    // 3:rd vay there is a preferred address set (and, a fourth alternative exists... do later )
                    shippingAddress = new OrderAddress(
                        CustomerContext.Current.CurrentContact.PreferredShippingAddress);
                }
            }

            return shippingAddress;
        }

        private void AdjustFirstShipmentInOrder(ICart cart, IOrderAddress orderAddress, Guid selectedShip)
        {
            // Need to set the guid (name is good to have too) of some "real shipmentment in the DB"
            // RoCe - this step is not needed, actually - code and lab-steps can be updated
            // We'll do it to show how it works
            var shippingMethod = ShippingManager.GetShippingMethod(selectedShip).ShippingMethod.First();

            IShipment theShip = cart.GetFirstShipment(); // ...as we get one "for free"

            // Need the choice of shipment from DropDowns
            theShip.ShippingMethodId = shippingMethod.ShippingMethodId;
            //theShip.ShippingMethodName = "TucTuc";

            theShip.ShippingAddress = orderAddress;

            #region Hard coded and cheating just to show

            // RoCe: - fix the MarketService
            var mSrv = ServiceLocator.Current.GetInstance<IMarketService>();
            var defaultMarket = mSrv.GetMarket(MarketId.Default); // cheating some
            Money cost00 = theShip.GetShippingCost(_currentMarket.GetCurrentMarket(), new Currency("USD"));
            Money cost000 = theShip.GetShippingCost(_currentMarket.GetCurrentMarket(), cart.Currency);
            #endregion

            Money cost0 = theShip.GetShippingCost(
                _currentMarket.GetCurrentMarket()
                , _currentMarket.GetCurrentMarket().DefaultCurrency); // to make it easy

            // done by the "default calculator"
            Money cost1 = theShip.GetShippingItemsTotal(_currentMarket.GetCurrentMarket().DefaultCurrency);

            theShip.ShipmentTrackingNumber = "ABC123";
        }

        // RoCe - this can also be simplified
        private void AddPaymentToOrder(ICart cart, Guid selectedPaymentGuid)
        {
            if (cart.GetFirstForm().Payments.Any())
            {
                // should maybe clean up in the cart here
            }

            var selectedPaymentMethod =
                PaymentManager.GetPaymentMethod(selectedPaymentGuid).PaymentMethod.First();

            var payment = _orderGroupFactory.CreatePayment(cart);

            payment.PaymentMethodId = selectedPaymentMethod.PaymentMethodId;
            payment.PaymentType = PaymentType.Other;
            payment.PaymentMethodName = selectedPaymentMethod.Name; // check if any string still works :) ... it does

            // RoCe: need to get the Diff from the firts payment
            // ...we also have - cart.GetTotal(_orderGroupCalculator)
            // ... this is from Fund. and the first (and only payment)
            payment.Amount = _orderGroupCalculator.GetTotal(cart).Amount; // need a debug here

            cart.AddPayment(payment);
            // could add payment.BillingAddress = theAddress ... if we had it here
        }

        #endregion

        #region InventoryExtras

        public void CheckOnInventory()
        {
            ICart cart = base.GetCart();
            IShipment theShipment = cart.GetFirstShipment();
            // ?? Clear Ops-Keys ?? Quan's book
            Shipment s = (Shipment)theShipment;
            // have to be a concrete
            s.ClearOperationKeys(); // OK

            ILineItem lineItem = theShipment.LineItems.FirstOrDefault();
            

        }


        #endregion

    }
}