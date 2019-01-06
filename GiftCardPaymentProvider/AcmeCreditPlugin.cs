using EPiServer.Commerce.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace GiftCardPaymentProvider
{
    public class AcmeCreditPlugin : IPaymentPlugin
    {
        public IDictionary<string, string> Settings { get; set; }

        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            decimal CreditLimit = 500;
            if(payment.Amount <= CreditLimit)
            {
                return PaymentProcessingResult.CreateSuccessfulResult("Acme credit approved payment!");
            }
            else
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult("Sorry, you are over your limit!");
            }
        }
    }
}
