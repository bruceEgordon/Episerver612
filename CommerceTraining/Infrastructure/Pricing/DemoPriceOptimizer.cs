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
            var code = prices.First().CatalogKey.CatalogEntryCode;

            if (code == "Long Sleeve Shirt White Small_1")
            {
                //var shirts = prices.GroupBy(p => new
                //{
                //    p.CatalogKey,
                //    p.MinQuantity,
                //    p.MarketId,
                //    p.ValidFrom,
                //    p.CustomerPricing,
                //    p.UnitPrice.Currency
                //})
                //.Select(g => g.OrderByDescending(c => c.UnitPrice.Amount)
                //.First()).Select(p => new OptimizedPriceValue(p, null));

                var shirts = prices.GroupBy(p => new
                {
                    p.MarketId,
                    p.CustomerPricing
                })
                .Select(g => g.OrderByDescending(c => c.UnitPrice.Amount).First())
                .Select(p => new OptimizedPriceValue(p, null));

                return shirts;
            }
            else return _defaultPriceOptimzer.OptimizePrices(prices);
        }
    }
}