using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Order.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class DemoPromoViewModel
    {
        public IEnumerable<RewardDescription> Rewards { get; set; }
        public InMemoryOrderGroup InMemCart { get; set; }
    }
}