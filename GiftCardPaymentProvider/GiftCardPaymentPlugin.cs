using System;
using System.Collections.Generic;
using EPiServer.Commerce.Order;

namespace GiftCardPaymentProvider
{
    public class GiftCardPaymentPlugin : IPaymentPlugin
    {
        public IDictionary<string, string> Settings { get; set; }

        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            return PaymentProcessingResult.CreateSuccessfulResult("Gift card payment accepted!");
        }
    }
}
