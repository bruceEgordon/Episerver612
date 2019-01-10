using CommerceTraining.Models.Catalog;
using EPiServer.Commerce.Order;
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class PaymentDemoViewModel
    {
        public Guid SelectedPaymentId { get; set; }
        public IEnumerable<PaymentMethodDto.PaymentMethodRow> PayMethods { get; set; }
        public ShirtVariation Shirt { get; set; }
        public string ImageUrl { get; set; }
        public DefaultVariation Suspenders { get; set; }
        public string SuspendersImageUrl { get; set; }
        public int PurchaseQuantity { get; set; }
        public Money CartTotal { get; set; }
        public IEnumerable<ILineItem> CartItems { get; set; }
        public IEnumerable<EntityObject> GiftCards { get; set; }
        public bool UseGiftCard { get; set; }
        public string RedemtionCode { get; set; }
        public decimal GiftCardDebitAmt { get; set; }
        public string MessageOutput { get; set; }
        public string GiftInfoMessage { get; set; }
    }
}