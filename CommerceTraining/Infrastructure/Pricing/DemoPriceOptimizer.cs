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
            if (shirtPrice != null && shirtPrice.Count() > 0)
            {
                return shirtPrice.GroupBy(p => new
                {
                    p.CatalogKey,
                    p.MinQuantity,
                    p.MarketId,
                    p.ValidFrom,
                    p.CustomerPricing,
                    p.UnitPrice.Currency
                })
                    .Select(g => g.OrderByDescending(c => c.UnitPrice.Amount)
                    .First()).Select(p => new OptimizedPriceValue(p, null));
            }
            else return _defaultPriceOptimzer.OptimizePrices(prices);
        }
    }
}