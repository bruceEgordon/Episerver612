using EPiServer.Commerce.Order;
using Mediachase.Commerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Infrastructure.CartAndCheckout
{
    public class DemoCustomTaxCalc : ITaxCalculator
    {
        private ITaxCalculator _defaultTaxCalculator;

        public DemoCustomTaxCalc(ITaxCalculator defaultTaxCalculator)
        {
            _defaultTaxCalculator = defaultTaxCalculator;
        }
        public Money GetReturnTaxTotal(IReturnOrderForm returnOrderForm, IMarket market, Currency currency)
        {
            throw new NotImplementedException();
        }

        public Money GetSalesTax(ILineItem lineItem, IMarket market, IOrderAddress shippingAddress, Money basePrice)
        {
            throw new NotImplementedException();
        }

        public Money GetShippingReturnTaxTotal(IShipment shipment, IMarket market, Currency currency)
        {
            throw new NotImplementedException();
        }

        public Money GetShippingTax(ILineItem lineItem, IMarket market, IOrderAddress shippingAddress, Money basePrice)
        {
            throw new NotImplementedException();
        }

        public Money GetShippingTaxTotal(IShipment shipment, IMarket market, Currency currency)
        {
            throw new NotImplementedException();
        }

        public Money GetTaxTotal(IOrderGroup orderGroup, IMarket market, Currency currency)
        {
            throw new NotImplementedException();
        }

        public Money GetTaxTotal(IOrderForm orderForm, IMarket market, Currency currency)
        {
            throw new NotImplementedException();
        }
    }
}