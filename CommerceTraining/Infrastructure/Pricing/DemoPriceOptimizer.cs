using Mediachase.Commerce.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Infrastructure.Pricing
{
    public class DemoPriceOptimizer : IPriceOptimizer
    {
        private DefaultPriceOptimizer _defaultPriceOptimzer;
        public DemoPriceOptimizer(DefaultPriceOptimizer defaultPriceOptimizer)
        {
            _defaultPriceOptimzer = defaultPriceOptimizer;
        }
        public IEnumerable<IOptimizedPriceValue> OptimizePrices(IEnumerable<IPriceValue> prices)
        {
            var shirtPrice = from price in prices
                             where price.CatalogKey.CatalogEntryCode == "Long Sleeve Shirt White Small_1"
                             select price;
            throw new NotImplementedException();
        }
    }
}